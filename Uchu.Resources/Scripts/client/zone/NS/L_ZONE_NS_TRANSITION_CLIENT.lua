local WORLD_VIGNETTE_LOT = 4884
local TRUCK_VIGNETTE_LOT = 4906
local CONTROLLER_VIGNETTE_LOT = 4907
local STARS_VIGNETTE_LOT = 4914
local PATH_NAMES = {}
PATH_NAMES[1]="Rocket_01_Path_01"
PATH_NAMES[2]="Rocket_02_Path_01"
PATH_NAMES[3]="Rocket_Path_01"
PATH_NAMES[4]="Rocket_04_Path_01"

function onStartupTransition(self)

end

function onChildLoadedTransition( self, msg )
	if (msg.templateID == 4983) then
        storeObjectByName(self, "customClone", msg.childID)
    elseif (msg.templateID == WORLD_VIGNETTE_LOT) then
        storeObjectByName(self, "vignetteObject", msg.childID)
    elseif (msg.templateID == STARS_VIGNETTE_LOT) then
        storeObjectByName(self, "vignetteStars", msg.childID)
    elseif (msg.templateID == TRUCK_VIGNETTE_LOT) then
        storeObjectByName(self, "vignetteTruck", msg.childID)
    elseif (msg.templateID == CONTROLLER_VIGNETTE_LOT) then
        storeObjectByName(self, "vignetteController", msg.childID)
    end
end

function onCinematicUpdateTransition(self, msg)
	if(msg.event == "STARTED") then
		if(msg.pathName == "Rocket_Path_04") then
        
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.01, "RocketSoar",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.05, "TruckPreload",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 11.33, "RocketPic",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 13, "FaceFocus",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 14.1, "RocketLand",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 19.5, "TruckBoom",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 22, "RocketBoom",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 26, "FaceChange",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 29, "RocketUnfreeze",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 29.49, "TelePlayer",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 29.52, "SnapPlayersCam",self )

        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
            player:SetPosition {pos = {x=526.3,y=94.6,z=-443.57}}
            player:SetRotation {x=0,y=-0.549,z=0,w=0.835}
		    canFire = true
		    RESMGR:LoadObject {objectTemplate = 4983,
                               x = 668.55,
                               y = 134.2,
                               z = -506.13,
                               rw = 0.835,
                               rx = 0,
                               ry = -0.549,
                               rz = 0,
                               owner = self}
			--print(tostring(self:GetVar("selection1")) .. " " .. tostring(self:GetVar("selection2")) .. " " .. tostring(self:GetVar("selection3")))
			RESMGR:LoadObject {objectTemplate = self:GetVar("selection1"),
                               owner = self}
			RESMGR:LoadObject {objectTemplate = self:GetVar("selection2"),
                               owner = self}
			RESMGR:LoadObject {objectTemplate = self:GetVar("selection3"),
                               owner = self}
                               


	    else 
            for i,v in ipairs(PATH_NAMES) do
                if msg.pathName == v then
                    RESMGR:LoadObject {objectTemplate = WORLD_VIGNETTE_LOT,
                                       x = 668.55,
                                       y = 134.2,
                                       z = -506.13,
                                       rw = 0.835,
                                       rx = 0,
                                       ry = -0.549,
                                       rz = 0,
                                       owner = self}
                    RESMGR:LoadObject {objectTemplate = STARS_VIGNETTE_LOT,
                                       x = 668.55,
                                       y = 134.2,
                                       z = -506.13,
                                       rw = 0.835,
                                       rx = 0,
                                       ry = -0.549,
                                       rz = 0,
                                       owner = self}
                    RESMGR:LoadObject {objectTemplate = TRUCK_VIGNETTE_LOT,
                                       x = 645.89,
                                       y = 134.2,
                                       z = -556.34,
                                       rw = 0.835,
                                       rx = 0,
                                       ry = -0.549,
                                       rz = 0,
                                       owner = self}
                    RESMGR:LoadObject {objectTemplate = CONTROLLER_VIGNETTE_LOT,
                                       x = 691.34,
                                       y = 134.33,
                                       z = -512.25,
                                       rw = 0.551,
                                       rx = 0,
                                       ry = 0.833,
                                       rz = 0,
                                       owner = self}
                end
            end        
        end
    end
end

function onNotifyObjectTransition(self, msg)
    if msg.name == "customRocketSelected1" then
		self:SetVar("selection1", msg.param1)
	elseif msg.name == "customRocketSelected2" then
		self:SetVar("selection2", msg.param1)
	elseif msg.name == "customRocketSelected3" then
		self:SetVar("selection3", msg.param1)		
    end
end

function onChildRenderComponentReadyTransition(self, msg)
	if msg.childLOT == self:GetVar("selection1") then
		storeObjectByName(self, "selection1Obj", msg.childID)
	elseif msg.childLOT == self:GetVar("selection2") then
		storeObjectByName(self, "selection2Obj", msg.childID)
	elseif msg.childLOT == self:GetVar("selection3") then
		storeObjectByName(self, "selection3Obj", msg.childID)
	end

	if getObjectByName(self, "selection1Obj") and getObjectByName(self, "selection2Obj") and getObjectByName(self, "selection3Obj") then
		getObjectByName(self, "customClone"):AttachObject{childID = getObjectByName(self, "selection3Obj"), subNodeName = "custom_attach_point"}
		getObjectByName(self, "selection3Obj"):AttachObject{childID = getObjectByName(self, "selection2Obj"), subNodeName = "CP_A1"}
		getObjectByName(self, "selection2Obj"):AttachObject{childID = getObjectByName(self, "selection1Obj"), subNodeName = "CP_B2"}
	end
end

function onTimerDoneTransition( self, msg )

    if (msg.name == "RocketSoar") then
        --print "Truck Hack"
        getObjectByName(self, "customClone"):PlayAnimation{animationID = "soar"}
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayAnimation{animationID = "rocket-soar"}
        player:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        getObjectByName(self, "vignetteObject"):PlayAnimation{animationID = "soar"}
        getObjectByName(self, "vignetteStars"):PlayAnimation{animationID = "soar"}
        getObjectByName(self, "vignetteController"):PlayAnimation{animationID = "soar"}        
        getObjectByName(self, "vignetteTruck"):PlayAnimation{animationID = "land"}

    end

    if (msg.name == "TruckPreload") then

		if getObjectByName(self, "selection1Obj") and getObjectByName(self, "selection2Obj") and getObjectByName(self, "selection3Obj") then
			getObjectByName(self, "selection3Obj"):PlayFXEffect{effectType = "launch"}
			getObjectByName(self, "selection2Obj"):PlayFXEffect{effectType = "launch"}
			getObjectByName(self, "selection1Obj"):PlayFXEffect{effectType = "launch"}
	    end

        --print "Truck Land"

        getObjectByName(self, "vignetteTruck"):PlayAnimation{animationID = "land"}

    end

    if (msg.name == "RocketPic") then
        --print "Picture Time"
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
     --   getObjectByName(self, "customClone"):PlayAnimation{animationID = "rocket-pic"}
	 --   local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
     --   player:PlayAnimation{animationID = "rocket-pic"}
    end

    if (msg.name == "FaceFocus") then
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
    end

	if (msg.name == "RocketLand") then
        --print "Landing Time"
     --   getObjectByName(self, "customClone"):PlayAnimation{animationID = "rocket-land"}
     --   local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        GAMEOBJ:DeleteObject(getObjectByName(self, "vignetteObject"))
        GAMEOBJ:DeleteObject(getObjectByName(self, "vignetteStars"))
     --   player:PlayAnimation{animationID = "rocket-land"}
    end

	if (msg.name == "TruckBoom") then
        getObjectByName(self, "vignetteTruck"):PlayAnimation{animationID = "boom"}
    end

	if (msg.name == "RocketBoom") then
        --print "BOOM!"
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:DeleteObject(getObjectByName(self, "selection1Obj"))
        getObjectByName(self, "selection2Obj"):DetachObject{}
        GAMEOBJ:DeleteObject(getObjectByName(self, "selection2Obj"))
        getObjectByName(self, "selection3Obj"):DetachObject{}
        GAMEOBJ:DeleteObject(getObjectByName(self, "selection3Obj"))

    end

	if (msg.name == "FaceChange") then
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayFaceDecalAnimation { animationID = "Blink", useAllDecals = true }
    end

	if (msg.name == "RocketUnfreeze") then
        --print "Frees Player"
        getObjectByName(self, "selection1Obj"):DetachObject{}
        GAMEOBJ:DeleteObject(getObjectByName(self, "vignetteTruck"))
        GAMEOBJ:DeleteObject(getObjectByName(self, "vignetteController"))
        GAMEOBJ:DeleteObject(getObjectByName(self, "customClone"))
    end

	if (msg.name == "TelePlayer") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetPosition {pos = {x=443.63,y=94.34,z=-485.2}}
        player:SetRotation {x=0,y=-0.873,z=0,w=0.486}
        player:SetUserCtrlCompPause{bPaused = false}
	end

	if (msg.name == "SnapPlayersCam") then
		CAMERA:SnapCameraToPlayer()
	end
end
