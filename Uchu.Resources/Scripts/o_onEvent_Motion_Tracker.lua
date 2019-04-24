
--------------------------------------------------------------
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end

--[[
function onProximityUpdate(self, msg)

 
     
    if (onTemplateProximityUpdate) and (onTemplateProximityUpdate(self, msg) == true ) then
    	return 
    end 		
    
    
	 
    if msg.status == "ENTER" and (isAttackable(self,msg) == true)  then
     
     if self:GetVar("Set.SuspendLuaAI") == true then
		setState("combat",self)
		return
	end
	
        local myPos = self:GetPosition().pos
        local myTarget = msg.objId
		CancelTemplateTimers(self)
	    storeObjectByName(self, "AttackingTarget", msg.objId)
	    storeHomePoint(self)
		self:SetTetherPoint { tetherPt = myPos, radius = self:GetVar("tetherRadius") }
		self:SetVar("aggrotarget",1) 
		
		if self:GetVar("Set.AggroEmote") and self:GetVar("AggroEmoteDelay") then
			-- face Target
			self:FaceTarget{ target = myTarget, degreesOff = 5, keepFacingTarget = true }
			-- Emote to Target
			Emote.emote(self,myTarget , self:GetVar("Set.AggroE_Type")) 
			-- Create timer that triggers the follow/attack
			GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.AggroE_Delay") , "AggroEmote", self )
			self:SetVar("AggroEmoteDelay", false) 
			setState("Idle",self)
			return

		end
	
			setState("aggro", self)
	end
 
end
--]]

function onCollisionPhantom(self, msg)
    local target = msg.objectID
    local faction = target:BelongsToFaction{factionID = 1}.bIsInFaction
    local playerVelx = target:GetLinearVelocity().linVelocity.x
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z
    if faction and playerVelx ~= 0 and playerVelz ~= 0 and self:GetVar("collideswitch") == 1 then
        self:NotifyClientObject{name = "Gotcha"}
        self:SetVar("collideswitch", 0)
        local myPos = self:GetPosition().pos
        local myTarget = msg.objId
		CancelTemplateTimers(self)
	    storeObjectByName(self, "AttackingTarget", target)
	    storeHomePoint(self)
		self:SetTetherPoint { tetherPt = myPos, radius = self:GetVar("tetherRadius") }
		self:SetVar("aggrotarget",1) 
        self:SetVar("pauseswitch", 1)
        setState("aggro", self)
        GAMEOBJ:GetTimer():CancelTimer( "CheckingforMovement", self )
        GAMEOBJ:GetTimer():AddTimerWithCancel(  0.5 , "CheckingforMovement", self )
    end
end

function ProximityPuls(self)
   -- print("ProximityPuls")
    self:SetVar("AttackingTarget", "NoTarget" )
    local foundObj = self:GetProximityObjects { name = "aggroRadius"}.objects
           self:SetVar("ProxFind", false)
            i = 1
     while (i <= table.maxn (foundObj) or (self:GetVar("ProxFind")== true)) do  
       -- print("Found ... "..table.maxn (foundObj).." Objects")
        if isOBJAttackable(self,foundObj[i]) then
            self:SetVar("ProxFind", true)
            local myPos = self:GetPosition().pos
            local myTarget = foundObj[i]
            CancelTemplateTimers(self)
            storeObjectByName(self, "AttackingTarget", foundObj[i])
         
            --self:SetTetherPoint { tetherPt = myPos, radius = self:GetVar("tetherRadius") }
            self:SetVar("aggrotarget",1) 

         
            if self:GetVar("Set.AggroEmote") and self:GetVar("AggroEmoteDelay") then
                -- face Target
                self:FaceTarget{ target = myTarget, degreesOff = 5, keepFacingTarget = true }
                -- Emote to Target
                Emote.emote(self,myTarget , self:GetVar("Set.AggroE_Type")) 
                -- Create timer that triggers the follow/attack
                GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.AggroE_Delay") , "AggroEmote", self )
                self:SetVar("AggroEmoteDelay", false) 
                setState("Idle",self)
             
            end
        end
         i = i + 1
        if (self:GetVar("ProxFind")== true) then
			if self:GetVar("Set.SuspendLuaAI") == true then
				setState("combat",self)
				return
			end
            setState("aggro", self)    
            break
        end
    end
        if (self:GetVar("ProxFind")== false) then
            setState("tether", self)    
        end
    
end

function onOnHit(self,msg)
    if (onTemplateHit) and (onTemplateHit(self, msg) == true ) then
    	return 
    end  

    if self:GetVar("Set.Aggression") ~= "Passive" and not self:GetVar("AiDisabled") then
        storeObjectByName(self, "AttackingTarget", msg.attacker)
        if not self:GetVar("inpursuit")  and not self:GetVar("AiDisabled") then
            storeHomePoint(self)
            self:SetTetherPoint { tetherPt = myPos,radius = self:GetVar("Set.tetherRadius") }
        end
        if self:IsEnemy{ targetID = msg.attacker }.enemy == true and not msg.attacker:IsDead().bDead and not self:IsDead{}.bDead and not self:GetVar("AiDisabled") then
            CancelTemplateTimers(self)
            storeObjectByName(self, "AttackingTarget", msg.attacker)
            local aggroTarget = self:GetVar("aggrotarget") 
            local myPos = self:GetPosition().pos
            setState("aggro", self)
            self:SetVar("aggrotarget",1) 
            GAMEOBJ:GetTimer():CancelTimer( "CheckingforMovement", self )
            GAMEOBJ:GetTimer():AddTimerWithCancel(  0.5 , "CheckingforMovement", self )
        end
    end
end
--/////////////////////////////////////////////////////////////////////////////////////////////////////////////
-- Timers
--////////////////////////////////////////////////////////////////////////////////////////////////////////////


onTimerDone = function(self, msg)
    local mytarget = getMyTarget(self)

    if (onTemplateTimerDone and  onTemplateTimerDone(self,msg) == true) then
           return
    end 
    
if not self:GetVar("AiDisabled")  then

    if msg.name == "DealyKillSelf" then
              self:Die{ killerID = self, killType = "SILENT" }
        	--self:KillObj{targetID = self}
    end
    
    if msg.name == "DealyAction" then
      n = self:GetVar("WPEvent_NUM") + 1
      self:SetVar("WPEvent_NUM", n ) 
      setState("WayPointEvent", self) 
    end

    if msg.name == "DelayActionEmote" then 
     n = self:GetVar("WPEvent_NUM") + 1
      self:SetVar("WPEvent_NUM", n ) 
      setState("WayPointEvent", self) 
    end 
    if msg.name == "EventMoveToDelete" then
    
        local objFound = {}
        local objFound = self:GetProximityObjects{ name = "KillOBJ" }.objects
        for i = 1 , table.maxn(objFound) do 
            if not objFound[i]:BelongsToFaction{factionID = 1}.bIsInFaction then
           
                self:Die{ killerID = objFound[i], killType = "SILENT" }
               --objFound[i]:MoveToDeleteQueue()
               
            end 
        end   
    
    end 
    if msg.name == "FaceDelay" then
        if self:GetVar("Con_Order") == "before" and self:GetVar("Con_Type") == "face" then
                setState("Emotes", self) 
            elseif self:GetVar("Con_Order") == "after" and self:GetVar("Con_Type") == "face" then
                setState("Actionface", self) 
            end  
      
    end
   
    if msg.name == "ActionDelay" then
        -- follow ----------------------------------------------------------------------------
        if  self:GetVar("Con_Type") == "follow" or self:GetVar("Con_Type") == "sneakto" then
            if self:GetVar("Con_Order") == "before" and self:GetVar("Con_Type") == "follow" or self:GetVar("Con_Type") == "sneakto" then
                setState("Emotes", self) 
            elseif self:GetVar("Con_Order") == "after" and self:GetVar("Con_Type") == "follow" or self:GetVar("Con_Type") == "sneakto" then
                setState("Actionfollow", self) 
            end  
        end 
    end 
    if msg.name == "EmoteDelay" then
        -- follow ----------------------------------------------------------------------------
        if  self:GetVar("Con_Type") == "follow" or self:GetVar("Con_Type") == "sneakto" then
 
            if self:GetVar("Con_Order") == "before" and self:GetVar("Con_Type") == "follow" or self:GetVar("Con_Type") == "sneakto" then
                    setState("Actionfollow",self)
                elseif self:GetVar("Con_Order") == "after" and self:GetVar("Con_Type") == "follow"  or self:GetVar("Con_Type") == "sneakto" then
                     setState("Emotes",self)
            end  
       end    
    end 
    if msg.name == "TimerDeleteObject" then 
	 self:Die{ killerID = self, killType = "SILENT" }
    end  
    if msg.name == "AggroEmote" then
        self:SetVar("AggroDelayDone", true) 
        if self:GetVar("Set.SuspendLuaAI") == true then
			setState("combat",self)
			return
		end
        setState("aggro",self)
    end 
    if msg.name == "PatDelay" then       -- Path Delay
        self:SetVar("delayDone",true)
        setState("patrolBegin", self)
    end
    if msg.name == "MeanderPause" then
        setState("Meander", self)
    end
 end
end

--*****************       On Die
function onLeftTetherRadius(self, msg)
    if (onTemplateTimerDone and  onTemplateTimerDone(self,msg) == true) then
           return
    end 

   self:SetVar("AttackingTarget", "NoTarget" )
   CancelTemplateTimers(self)
   getVarables(self) -- Reset all Saved Varables
   self:SetVar("tetherON", true)
   hp = self:GetMaxHealth{}.health
   self:SetHealth{ health = hp }
   setState("tether",self) 

end


function isAttackable(self,msg)

	if msg.objType == "Enemies" or msg.objType == "NPC" or msg.objType == "Rebuildables" then

		if self:IsEnemy{ targetID = msg.objId }.enemy and not msg.objId:IsDead().bDead and msg.name == "aggroRadius" then
		
			if self:GetVar("Set.Aggression") == "Aggressive" or self:GetVar("Set.Aggression") == "PassiveAggres" then
			
				if self:GetVar("aggrotarget") ~= 2 and self:GetVar("AttackingTarget") == "NoTarget" and not self:GetVar("AiDisabled") then

					return true
					
				end 
			end
		end 
		
	end
	
	return false 
	
end 
function isOBJAttackable(self,obj)

	if obj:GetType().objType == "Enemies" or obj:GetType().objType == "NPC" or obj:GetType().objType == "Rebuildables" then

		if self:IsEnemy{ targetID = obj }.enemy and not obj:IsDead().bDead then
		
			if self:GetVar("Set.Aggression") == "Aggressive" or self:GetVar("Set.Aggression") == "PassiveAggres" then
			
				if self:GetVar("aggrotarget") ~= 2 and self:GetVar("AttackingTarget") == "NoTarget" and not self:GetVar("AiDisabled") then

					return true
					
				end 
			end
		end 
		
	end
	
	return false 
	
end 

function onFireEvent(self, msg)
      local s = msg.args
      local t = split(s, ',')
      -- t[1]= name
      -- t[2]= pathname\
      -- t[3]= path num
    
    
    
    
    if t[1] == "startpath" then
       
            self:SetVar("attached_path", t[2])
            self:SetVar("attached_path_start", t[3] )
            self:FollowWaypoints{ bUseNewPath = true, newPathName = t[2], newStartingPoint = t[3] }
    end
    

end

function onResetScriptedAIState(self, msg)
	SetResetState(self)
end



