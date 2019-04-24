--------------------------------------------------------------
--pillar 3 script for gauntlet level
--------------------------------------------------------------

function onStartup(self)
   --self:AddObjectToGroup{name = "pillar"}
   --print("pillar 3 starting up!")
end

function onDie(self, msg)
   --print("pillar 3 destroyed!")
   local friends = self:GetObjectsInGroup{group = "pillar", ignoreSpawners = true}.objects[1]
   if friends then
      friends:NotifyObject{name = "P_3_Destroyed", ObjIDSender = self}
   end
end