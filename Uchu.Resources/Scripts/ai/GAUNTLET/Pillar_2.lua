--------------------------------------------------------------
--pillar 2 script for gauntlet level
--------------------------------------------------------------

function onStartup(self)
   --self:AddObjectToGroup{name = "pillar"}
   --print("pillar 2 starting up!")
end

function onDie(self, msg)
   --print("pillar 2 destroyed!")
   local friends = self:GetObjectsInGroup{group = "pillar", ignoreSpawners = true}.objects[1]
   if friends then
      friends:NotifyObject{name = "P_2_Destroyed", ObjIDSender = self}
   end
end