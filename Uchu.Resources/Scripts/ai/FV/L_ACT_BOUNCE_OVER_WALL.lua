----------------------------------------------------------------------
--updates achievement when player passes through a volume
----------------------------------------------------------------------



function onCollisionPhantom(self, msg)

   --print("collided")
   
   local player = msg.objectID
   
   player:UpdateMissionTask{taskType = "complete", value = 849, value2 = 1, target = self}

end