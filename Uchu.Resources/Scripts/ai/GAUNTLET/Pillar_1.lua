--------------------------------------------------------------
--pillar 1 script for gauntlet level
--------------------------------------------------------------

function onStartup(self)
   --self:AddObjectToGroup{name = "pillar"}
   --print("pillar 1 starting up!")
end

function onDie(self, msg)
   --print("pillar 1 destroyed!")
   local friends = self:GetObjectsInGroup{group = "pillar", ignoreSpawners = true}.objects[1]
   if friends then
      friends:NotifyObject{name = "P_1_Destroyed", ObjIDSender = self}
   end
end