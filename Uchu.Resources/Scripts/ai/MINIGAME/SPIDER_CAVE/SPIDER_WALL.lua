--------------------------------------------------
--script to attach to the spider wall that spawns a quick build when destroyed
--------------------------------------------------


function onStartup (self)
   --print("starting up the spider wall!")
end

--[[function onRebuildNotifyState (self, msg)
   --print(tostring(msg.iState))
   if (msg.iState == 2) then
      local friends = self:GetObjectsInGroup{group = "cannonWall", ignoreSpawners = true}.objects
      --print("wall ready!")
      for i, wall in pairs(friends) do
         if wall:GetLOT().objtemplate == 4956 then
            wall:NotifyObject{name = "wallIsBuilt", ObjIDSender = self}
            --print("notifying switch")
         end
      end
   end
end

function onDie (self, msg)
   local friends = self:GetObjectsInGroup{group = "cannonWall", ignoreSpawners = true}.objects
   --print("wall down!")
   for i, wall in pairs(friends) do
      if wall:GetLOT().objtemplate == 4956 then
         wall:NotifyObject{name = "wallDown", ObjIDSender = self}
      end
   end
end--]]

function onDie(self, msg)

    print("spider wall down!")
	local mypos = self:GetPosition().pos
    local posString = self:CreatePositionString{ x = mypos.x - 10, y = mypos.y, z = mypos.z - 20 }.string
	local myRot = self:GetRotation()
	local parent = msg.killerID;
	local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", 5}, {"no_timed_spawn", false}, {"CheckPrecondition" , "0:21"} }
	RESMGR:LoadObject { objectTemplate = 6483, x= mypos.x, y= mypos.y , z= mypos.z, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config, owner = parent }
	        
end