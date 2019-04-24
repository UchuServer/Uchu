--------------------------------------------------------------
-- Cinematic update handler
--------------------------------------------------------------
function onCinematicUpdate(self, msg)

	if(msg.event == "STARTED") then
		print("Path: " .. msg.pathName .. " started")
		-- RemoveAllCameraEffects()
		if(msg.pathName == "C") then
			LookCameraAtObject(GAMEOBJ:GetLocalCharID())
		end
	elseif(msg.event == "WAYPOINT") then
		print("Path: " .. msg.pathName .. " reached waypoint " .. msg.waypoint)
		print("Path Time / Overall: " .. msg.pathTime .. " / " .. msg.overallTime)
		if(msg.pathName == "B" and msg.waypoint == 1) then
			ShakeCamera()
	    end
	    if(msg.pathName == "C" and msg.waypoint == 5) then
		    -- RemoveCameraEffect("lookitMe!!")
	    end
	elseif(msg.event == "ENDED") then
		print("Path: " .. msg.pathName .. " ended")
	else
	    print("What the hoof?")
	end

end

--------------------------------------------------------------
-- Plays a short shake effect on the camera
--------------------------------------------------------------
function ShakeCamera()
	local config = { {"posFrequency", 24}, {"rotFrequency", 2}, {"xAmplitude", 4}, {"zRotation", 5} }
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):AddCameraEffect{ effectType = "shake", effectID = "omgMyShaek", duration = 5, configData = config }
end

--------------------------------------------------------------
-- Attaches a look-at effect to the camera
--------------------------------------------------------------
function LookCameraAtObject(object)
	local config = { {"objectID", "|" .. object}, {"leadIn", 5}, {"leadOut", 5}, {"lag", 0.85}, {"FOV", 10} }
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):AddCameraEffect{ effectType = "lookAt", effectID = "lookitMe!!", duration = 30, configData = config }
end

--------------------------------------------------------------
-- Removes a single camera effect from the local player
--------------------------------------------------------------
function RemoveCameraEffect(removeID)
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):RemoveCameraEffect{ effectID = removeID }
end

--------------------------------------------------------------
-- Removes all camera effects from the local player
--------------------------------------------------------------
function RemoveAllCameraEffects()
	GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):RemoveAllCameraEffects{}
end
