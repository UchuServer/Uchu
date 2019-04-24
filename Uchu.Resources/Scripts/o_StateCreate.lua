function oStart(self)
      self:SetVar("AiDisabled", false) 
   -- ////////////////////////////////////////////////////////////////////////////
      -- Aggro
      if self:GetVar('Set.Aggression') == "Aggressive" then
         self:SetProximityRadius { radius = self:GetVar("Set.aggroRadius") , name = "aggroRadius" }
         --if self:GetVar('Set.UseAggroFOV') then
         --   self:SetProximityRadius { radius = self:GetVar("Set.aggroFOVRadius") , name = "aggroFOVRadius" }
         --end
      end
    self:UseStateMachine{}  
   -- Create States    
   -- ////////////////////////////////////////////////////////////////////////////
      -- Basic State
      addState(Dead, "Dead", "Dead", self)
      addState(Idle, "Idle", "Idle", self)
      addState(tether, "tether", "tether", self)
      addState(GoHome, "GoHome" , "GoHome", self)
      addState(AiDisable, "AiDisable" , "AiDisable", self)
      addState(AiEnable, "AiEnable" , "AiEnable", self)
      addState(combat, "combat" , "combat" , self)
       if (onTemplateCustomStateEnter) or (onTemplateCustomStateArrived) then
	  	       addState(customState, "customState" , "customState", self)	   
	   end 
    
      
    

       
      -- Wander 
      if self:GetVar("Set.SuspendLuaMovementAI") == nil or self:GetVar("Set.SuspendLuaMovementAI") == false then
		  if self:GetVar("Set.MovementType") == "Wander" then
			  addState(Meander, "Meander", "Meander", self)
			  addState(aggro, "aggro", "aggro", self)
			  if self:GetVar("Set.FollowActive") then
				 addState(Follow, "Follow", "Follow", self)
			  end  

				beginStateMachine("Meander", self) 
		  end
		  -- Guard
		  if self:GetVar("Set.MovementType") == "Guard" then
				addState(aggro, "aggro", "aggro", self)
			   if self:GetVar("Set.FollowActive") then
				 addState(Follow, "Follow", "Follow", self)
			  end         
				beginStateMachine("Idle", self)
				Idle.onEnter(self) 
		  end 
	  else
		  beginStateMachine("Idle", self)
		  Idle.onEnter(self)
      end -- end suspend movement AI
      -- Guard
      if self:GetVar("Set.MovementType") == "Frozen" then
         addState(aggro, "aggro", "aggro", self)
         addSubState(attack, "attack", "attack", self)
         beginStateMachine("Idle", self)
         Idle.onEnter(self) 
      end
        -- Way Point Event ---  
      if self:GetVar("Set.WayPointSet") ~= nil or self:GetVar("attached_path") ~= nil or self:GetVar("groupID") ~= nil then
		if self:GetVar("Set.SuspendLuaMovementAI") == true then
			return
		end
            addState(WayPointEvent, "WayPointEvent", "WayPointEvent", self)
            if self:GetVar("groupID") == nil then
                self:FollowWaypoints()
            end

      end    
end
