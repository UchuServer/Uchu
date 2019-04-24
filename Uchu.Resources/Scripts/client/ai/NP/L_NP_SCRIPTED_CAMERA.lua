-- param1 = scene number
function onNotifyObject(self, msg)
	
	if msg.name == "startCam" then

		-- get path
		local path = self:GetVar("camPath")
		if (path) and (msg.param1 > 0) then

			-- save scene
			self:SetVar("SceneNum", msg.param1)
			
			-- set camera up
			self:SetMovingPlatformParams{ wsPlatformPath = path, iStartIndex = 0 }
			CAMERA:ActivateCamera("CAMERA_ATTACHED")
			CAMERA:AttachCameraToObj("CAMERA_ATTACHED", self, true, true)
			CAMERA:SetRenderCamera("CAMERA_ATTACHED")
			
		end
		
	end
	
end


function onPlatformAtLastWaypoint(self, msg)
	-- return to prev camera
	CAMERA:SetToPrevGameCam()

	-- notify zone object that we are done	
	local sceneNum = self:GetVar("SceneNum")
	if (sceneNum) then
		GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_" .. sceneNum .. "_end" }
	end
end


-- hide self on render
function onRenderComponentReady(self, msg)
	self:SetVisible{ visible = false }
end