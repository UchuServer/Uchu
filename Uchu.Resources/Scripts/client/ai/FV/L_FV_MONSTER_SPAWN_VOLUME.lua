-------------------------------------------------------------------------
--spawns in the monster at the FV gate
-------------------------------------------------------------------------



function onStartup(self)

   self:SetVar("HaveSpawned", false)
   self:AddObjectToGroup{group = "MonsterSpawnVolume"}
   
end


function onCollisionPhantom(self, msg)

   --print("collided")
   
   if self:GetVar("HaveSpawned") == false then
   
      self:SetVar("HaveSpawned", true)
      
      local spawnerobj = self:GetObjectsInGroup{group = "MonsterSpawn", ignoreSpawners = true}.objects[1]
      
      if spawnerobj then
      
         local pos = spawnerobj:GetPosition().pos
         local rot = spawnerobj:GetRotation()
         local config = {{"groupID", "Monster" }}
         RESMGR:LoadObject { objectTemplate = 9503  , x = pos.x , y =  pos.y , z = pos.z , rw = rot.w, rx = rot.x , ry = rot.y , rz = rot.z , owner = self,configData = config }
         
      end
      
      local gate = self:GetObjectsInGroup{group = "Gate", ignoreSpawners = true}.objects[1]
      
      if gate then
      
         gate:NotifyObject{name = "GateDown"}
         
      end
   
   end

end



function onNotifyObject(self, msg)

   if msg.name == "MonsterDespawned" then
      
      self:SetVar("HaveSpawned", false)
   
   end

end