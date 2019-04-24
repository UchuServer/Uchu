

function onMissionDialogueOK(self, msg)
    print ("Mission Dialog accepted!")
        
    --local targetID = msg.objId --GGJ Define targetID as msg.objID (the thing that sent the message, which is hopefully the player)
    local player = msg.responder --GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	player:SetFlag{iFlagID = 1, bFlag = true}
   
end

