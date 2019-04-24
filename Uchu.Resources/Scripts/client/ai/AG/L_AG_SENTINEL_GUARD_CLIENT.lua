-----------------------------------------------------------------------------------------------
--script so the player will always be able to rebuild the beacons once the mission is accepted
-----------------------------------------------------------------------------------------------
function onStartup(self)
   --print("client script starting up!!!!")
end


local BeaconQB = 55


function onMissionDialogueOK(self, msg)

--print("mission accepted")
   
   if msg.missionID == 322 then
      
      local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
      player:SetFlag{iFlagID = BeaconQB, bFlag = true}
      --print("flag set")
   end
end