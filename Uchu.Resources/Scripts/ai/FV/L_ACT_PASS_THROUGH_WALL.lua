----------------------------------------------------------------------
--updates achievement when player passes through a volume
----------------------------------------------------------------------



function onCollisionPhantom(self, msg)

   --print("collided")
   
   local player = msg.objectID
   
   if player:CheckPrecondition{ PreconditionID = 53,iPreconditionType = 0 }.bPass then
       player:UpdateMissionTask{taskType = "complete", value = 848, value2 = 1, target = self}
   end   

end