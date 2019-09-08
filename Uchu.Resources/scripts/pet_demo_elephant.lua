require('o_mis')


onArrived = function(self, msg)

    if self:GetVar("PathingActive") == nil or  self:GetVar("PathingActive") == true then
            if not msg.isLastPoint then
               self:SetVar("SavedWP", msg.wayPoint)
               self:SetVar("PathName", msg.pathName) 
            else
               self:SetVar("SavedWP", 0)
               self:SetVar("PathName", msg.pathName) 
            end
            if (msg.actions) then  


                -- Clear Last Event Set if true
                if self:GetVar("Act_V") or self:GetVar("Act_N") then
                          
                         for  c = 1, table.maxn(self:GetVar("Act_N")) do
                            self:SetVar("Act_N."..c, nil)
                         end
                         for  c = 1, table.maxn(self:GetVar("Act_V")) do
                            self:SetVar("Act_V."..c, nil)
                         end
                end 
                -- Store Events
                    local  Act_N = {}
                    local  Act_V = {}
                     for i = 1, table.maxn(msg.actions) do
                         Act_N[i] = msg.actions[i].name    
                         Act_V[i] = msg.actions[i].value   
                        if  msg.actions[i].value == nil then
                             Act_V[i] = "noValue" 
                        end
                       
                     end
                    self:SetVar("Act_V",Act_V)
                    self:SetVar("Act_N",Act_N)
                   UseWayPoints(self)

            else
                         
            
                self:ContinueWaypoints()
            end
    end
end

-- Event Wp State --- 
function UseWayPoints(self)

   
    local o = self:GetVar("WPEvent_NUM") 
         if self:GetVar("WPEvent_NUM") <= table.maxn(self:GetVar("Act_N")) then
         
           -- o = o + 1
              if  self:GetVar("Act_V")  == "noValue"  then
                WayPointEventFunc(self, self:GetVar("Act_N")[o],self:GetVar("Act_V"))
              else
                
                WayPointEventFunc(self, self:GetVar("Act_N")[o],self:GetVar("Act_V")[o])
              end
         else
           self:SetVar("WPEvent_NUM", 1)
          self:ContinueWaypoints()
         end 
    

end 



function WayPointEventFunc(self,name,value)
    if name == "reset" then
        self:SetVar("PathingActive", false) 
        self:RemovePetState{ iStateType = 10 }
    end 
    
    if name == "stop" then
     self:StopMoving()
    end 
    
    
    if name == "groupemote" then
      local s = value
      local t = split(s, ',')
      local friends = self:GetObjectsInGroup{ group = t[1] }.objects
      
        for i = 1, table.maxn (friends) do 

            for b = 3, table.maxn(t) do
                if friends[i]:GetLOT().objtemplate == tonumber(t[b]) then -- Mim
                    Emote.emote(friends[i], friends[i], t[2] )
                end 
            end              
        end 
      
         self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
         UseWayPoints(self) 
    
    end 
    
    
    
    if name == "setvar" then
            local s = value
            local t = split(s, ',')
            self:SetVar(t[1],t[2])
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            UseWayPoints(self) 
    end 
    
     if name == "castskill" then
     
            if value ~= "noValue" then
                self:CastSkill{skillID = value } 
                self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
                UseWayPoints(self)
         
            else
                self:CastSkill{skillID = self:GetSkills().skills[1] } 
                self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
                UseWayPoints(self)
            
            end
    
     end 

      if name == "eqInvent" then
          local meItem = self:GetInventoryItemInSlot().itemID
		  self:EquipInventory{ itemtoequip = meItem}
		  
          self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
          UseWayPoints(self)
          
      end
      if name == "unInvent" then
          local meItem = self:GetInventoryItemInSlot().itemID
		  self:UnEquipInventory{ itemtounequip = meItem}
		  
          self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
          UseWayPoints(self)
          
      end

       ----------------------- delay
       if name == "delay" then
          self:StopMoving()
          GAMEOBJ:GetTimer():AddTimerWithCancel( value , "DealyAction", self )
       end 
      ----------------------- emote
      if name == "femote" then
        Emote.emote(self, self , value )
        self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
        UseWayPoints(self) 
      end
      
      if name == "emote" then
            Emote.emote(self, self , value )
            local time = self:GetAnimationTime{  animationID = value }.time
            if time < 1 then 
                time = 1
            end
            GAMEOBJ:GetTimer():AddTimerWithCancel( time  , "DelayActionEmote", self )
      end 
      ----------------------- teleport
      if name == "teleport" then
            local s = value
            local t = split(s, ',')

            local xSplit= {x=t[1] , y=t[2] , z=t[3] }
            self:Teleport{ pos = xSplit}
            
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            UseWayPoints(self) 
      end
      ----------------------- walkspeed
      if name == "pathspeed" then 
        self:SetPathingSpeed{speed = value}
        self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
        UseWayPoints(self) 
      end 
      ----------------------- removeOBJ
      if name == "removeNPC" then -- remove Objects in X
         local foundObj = self:GetProximityObjects { name = "KillOBJS"}.objects
              for i = 1, table.maxn (foundObj) do  
                if foundObj[i]:GetFaction().faction == value then
                   foundObj[i]:MoveToDeleteQueue()
                
                end
                
              end  
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            UseWayPoints(self) 
       end
       ----------------------- - Change WP
       if name == "changeWP" then-- 1 
         

            if string.find (value, ",") then
             self:SetVar("isTable", true ) 
            else
             self:SetVar("isTable", false ) 
            end 
                 
            if (self:GetVar("isTable")) then
                 local s = value
                 local t = split(s, ',')
                 
                self:SetVar("WPEvent_NUM", 1)  
                self:SetVar("attached_path", t[1])
                self:SetVar("attached_path_start", t[2] )
                self:FollowWaypoints()
         
            else
                self:SetVar("WPEvent_NUM", 1)  
                self:SetVar("attached_path", value)
                self:SetVar("attached_path_start", 0 )
                self:FollowWaypoints()
            
            end
        
       end
        ----------------------- - killSelf
        if name == "killself" and value == "noValue" then -- 2 - Kill Self
            self:Die{ killerID = self, killType = "SILENT" }
         elseif name == "killself" and value >=1  then 
            GAMEOBJ:GetTimer():AddTimerWithCancel( value , "DealyKillSelf", self )
        end 
        ----------------------- - killSelf
        if name == "removeself" then-- 2 - Kill Self
           	self:MoveToDeleteQueue()
        end 
        ----------------------- - spawn Object
        if event == "spawnOBJ" then  -- 3 - Spawn Object
              RESMGR:LoadObject { objectTemplate = value , x = self:GetPosition().pos.x , y = self:GetPosition().pos.y , z = self:GetPosition().pos.z ,owner = self } 
              self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
              UseWayPoints(self)             
        end 

end

onTimerDone = function(self,msg)

        if msg.name == "DealyAction" then
          n = self:GetVar("WPEvent_NUM") + 1
          self:SetVar("WPEvent_NUM", n ) 
          UseWayPoints(self)
        end

        if msg.name == "DelayActionEmote" then 
         n = self:GetVar("WPEvent_NUM") + 1
          self:SetVar("WPEvent_NUM", n ) 
         UseWayPoints(self)
        end 

        if msg.name == "DealyKillSelf" then
          self:Die{ killerID = self, killType = "SILENT" }
        end

end

