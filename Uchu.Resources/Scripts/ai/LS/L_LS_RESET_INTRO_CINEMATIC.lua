------------------------------------------------------------------------
--Script by Steve Y -- 6/23/10
--Sets the player flag 112 for the LS intro cinematic on the player to false when the player leaves
------------------------------------------------------------------------


function onUse(self, msg)
   
   local player = msg.user
   
   player:SetFlag { iFlagID = 112, bFlag = false }
   player:TransferToZone {zoneID =  1200, pos_x = -330, pos_y = 288, pos_z = 210, rot_x = 145}

end