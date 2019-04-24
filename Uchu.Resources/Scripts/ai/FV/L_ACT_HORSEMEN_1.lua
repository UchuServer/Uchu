--------------------------------------------------------
--horsemen 1 script
--------------------------------------------------------

function onStartup(self)
    self:SetVar("Set.SuspendLuaMovementAI", true)   -- a state suspending scripted movement AI
    
    self:SetVar("Set.MovementType", "Wander")       -- this is how the NPC will behave when not on a path. 
    -- Wander Settings ---------------------------------------------------------
    self:SetVar("Set.WanderChance",100)             -- Main Weight
    self:SetVar("Set.WanderDelayMin",5)             -- Min Wander Delay
    self:SetVar("Set.WanderDelayMax", 5)            -- Max Wander Delay
    self:SetVar("Set.WanderSpeed",1)                -- Move speed 
    self:SetVar("Set.wanderRadius",5)               -- Move radius 
    
    --print("horseman 1 starting up")
    --print( tostring(self:GetVar("groupID")) )
   
   for groupName in string.gmatch(self:GetVar("groupID"), "%w+;") do
      
      --------------------------------------------------------------
      --get the name of the group that the object is in and trim off the ';'s
      --------------------------------------------------------------
      
      groupName = string.sub(groupName, 1, -2)
      local mygroup = self:GetObjectsInGroup{group = groupName, ignoreSpawners = true}.objects
      
      --------------------------------------------------------------
      --for the object in my group with ID 8551, tell it I spawned
      --------------------------------------------------------------
      
      for i, object in ipairs(mygroup) do
         if object and object:GetLOT().objtemplate == 8551 then
   
            object:FireEvent{args = "ISpawned", senderID = self}
            --print("telling the turret I spawned")
         end
      end
   end
end

-- on horseman death
function onDie(self,msg)
	
	-- if Brick Fury killed the horseman
	if msg.killerID:GetLOT().objtemplate == 8665 then
		
		-- get the mission update volume by group name, tell it Brick fury killed the horseman
		local volume = self:GetObjectsInGroup{group = "HorsemenTrigger", ignoreSpawners = true}.objects[1]
		--I'm assuming there is only one volume
		if volume then
			volume:FireEvent{senderID = self; args = "HorsemanDeath"}
		end
	end
end