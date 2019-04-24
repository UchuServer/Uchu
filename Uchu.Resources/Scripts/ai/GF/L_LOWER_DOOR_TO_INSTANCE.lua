

function moveDoor(obj, goForward)
    local doorObj = obj:GetObjectsInGroup{ group = "Fire_Puzzle_Mover" }.objects
    for k,v in ipairs(doorObj) do        
        v:StartPathing()
		--print ("dont know why it isnt pathing")
    end
end

function onTimerDone(self, msg)
    if msg.name == "DoorUp" then
		moveDoor(self, false)
		--print ("door should be moving up")
	end
end

function onMissionDialogueOK(self, msg)
	if msg.bIsComplete == true and msg.missionID == 331 then  
		--print ("turned in mission")
		moveDoor(self, true)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 28, "DoorUp", self )
		local player = msg.responder
		if (player:GetFlag{iFlagID = 24}.bFlag == true) then return end  
        player:SetFlag{iFlagID = 24, bFlag = true}  

	end
end