
function onArrived(self, msg)
	if self:GetVar("Set.SuspendLuaMovementAI") == true then
		return
	end

if not self:GetVar("AiDisabled") then
        if not msg.isLastPoint then
           self:SetVar("SavedWP", msg.wayPoint)
           self:SetVar("PathName", msg.pathName) 
        else
           self:SetVar("SavedWP", nil)
           self:SetVar("PathName", nil) 
           self:SetVar("attached_path", nil)
        end
    if (msg.actions) and not self:GetVar("AiDisabled") then  


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
            setState("WayPointEvent", self) 



    else
                 
    
        self:ContinueWaypoints()
    end
   end
end

-- Event Wp State --- 
function UseWayPoints(self)
	if self:GetVar("Set.SuspendLuaMovementAI") == true then
		return
	end

    WayPointEvent = State.create()
    WayPointEvent.onEnter = function(self)
     self:SetVar("CurrentState", "WayPointEvent")
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

end 



function WayPointEventFunc(self,name,value)
    
	if self:GetVar("Set.SuspendLuaMovementAI") == true then
		return
	end

    
    if name == "bounce" then
       local  tpos =  LEVEL:GetPathWaypoints(self:GetVar("attached_path"))[(self:GetVar("SavedWP") + 2)].pos
       self:BouncePlayer{ niDestPt = tpos , fSpeed = value , ObjIDBouncer =  self }
       self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
       setState("WayPointEvent", self) 
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
         setState("WayPointEvent", self) 
    
    end 
    
    
    
    if name == "setvar" then
            local s = value
            local t = split(s, ',')
            self:SetVar(t[1],t[2])
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            setState("WayPointEvent", self) 
    end 
    
     if name == "castskill" then
     
            if value ~= "noValue" then
                self:CastSkill{skillID = value } 
                self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
                setState("WayPointEvent", self)
         
            else
                self:CastSkill{skillID = self:GetSkills().skills[1] } 
                self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
                setState("WayPointEvent", self)
            
            end
    
     end 

      if name == "eqInvent" then
          local meItem = self:GetInventoryItemInSlot().itemID
		  self:EquipInventory{ itemtoequip = meItem}
		  
          self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
          setState("WayPointEvent", self)
          
      end
      if name == "unInvent" then
          local meItem = self:GetInventoryItemInSlot().itemID
		  self:UnEquipInventory{ itemtounequip = meItem}
		  
          self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
          setState("WayPointEvent", self)
          
      end

       ----------------------- delay
       if name == "delay" then
          self:StopMoving()
          --GAMEOBJ:GetTimer():AddTimerWithCancel( value , "DealyAction", self )
          SetStoreTimmer(value,"DealyAction",self)
       end 
      ----------------------- emote
      if name == "femote" then
        Emote.emote(self, self , value )
        self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
        setState("WayPointEvent", self) 
      end
      
      if name == "emote" then
            Emote.emote(self, self , value )
            local time = self:GetAnimationTime{  animationID = value }.time
            if time < 1 then 
                time = 1
            end
           -- GAMEOBJ:GetTimer():AddTimerWithCancel( time  , "DelayActionEmote", self )
           SetStoreTimmer(time,"DelayActionEmote",self)
      end 
      ----------------------- teleport
      if name == "teleport" then
            local s = value
            local t = split(s, ',')

            local xSplit= {x=t[1] , y=t[2] , z=t[3] }
            self:Teleport{ pos = xSplit}
            
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            setState("WayPointEvent", self) 
      end
      ----------------------- walkspeed
      if name == "pathspeed" then 
        self:SetPathingSpeed{speed = value}
        self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
        setState("WayPointEvent", self) 
      end 
      ----------------------- removeOBJ
      if name == "removeNPC" then -- remove Objects in X
         local foundObj = self:GetProximityObjects { name = "KillOBJS"}.objects
              for i = 1, table.maxn (foundObj) do  
                if foundObj[i]:BelongsToFaction{factionID = value}.bIsInFaction then
                   foundObj[i]:MoveToDeleteQueue()
                end
                
              end  
            self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
            setState("WayPointEvent", self) 
       end
       ----------------------- - Change WP
       if name == "changeWP" then-- 1 
         
            -- trigger a callback for waypoint changing
            if (onTemplateChangeWaypoints) and (onTemplateChangeWaypoints(self, msg) == true ) then
                return 
            end             
            

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
                self:FollowWaypoints{ bUseNewPath = true, newPathName = t[1], newStartingPoint = t[2] }
         
            else

                self:SetVar("WPEvent_NUM", 1)  
                self:SetVar("attached_path", value)
                self:SetVar("attached_path_start", 0 )
				self:FollowWaypoints{ bUseNewPath = true, newPathName = value, newStartingPoint = 0 }            
				
            end
        
       end
       if name == "DeleteSelf" then
         GAMEOBJ:DeleteObject(self)
       end 
       
        ----------------------- - killSelf
        if name == "killself" and value == "noValue" then -- 2 - Kill Self
            self:Die{ killerID = self, killType = "SILENT" }
         elseif name == "killself" and value >=1  then 
            --GAMEOBJ:GetTimer():AddTimerWithCancel( value , "DealyKillSelf", self )
            SetStoreTimmer(value,"DealyKillSelf",self)
        end 
        ----------------------- - killSelf
        if name == "removeself" then-- 2 - Kill Self
           	self:MoveToDeleteQueue()
        end 
        ----------------------- - spawn Object
        if name == "spawnOBJ" then  -- 3 - Spawn Object
              --RESMGR:LoadObject { objectTemplate = value , x = self:GetPosition().pos.x , y = self:GetPosition().pos.y , z = self:GetPosition().pos.z ,owner = self } 
    
            local mypos = self:GetPosition().pos
            local myRot = self:GetRotation()
            self:Die{killType = "SILENT"} 
            if self:GetVar("pathset") ~= nil then
                  local config = { {"pathset", self:GetVar("pathset") } ,{"SpawnedVar", self:GetVar("SpawnedVar") } }
                  RESMGR:LoadObject { objectTemplate = value, x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config } 
            else
                  RESMGR:LoadObject { objectTemplate = value, x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z }
            end
              
              self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
              setState("WayPointEvent", self)             
        end 

end





