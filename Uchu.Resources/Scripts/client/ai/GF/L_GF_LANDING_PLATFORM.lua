require('o_mis')

--local bHasBeenUsed = false

local bCINEMA_ONCE = false


function onStartup(self)
    --print ("Cine Script Started!")

    -- Set variable on self to remember if the Rancher has been found
    --self:SetVar("RancherFound", false)

end


-- Something touches the phantom object
function onCollisionPhantom(self, msg)
    --print ("Collision Detected")

    -- define player
    local playerID = GAMEOBJ:GetLocalCharID()
    --print (playerID)
    --print (msg.objectID:GetID())

    if (msg.objectID:GetID() == playerID) then
    --print ("playerID triggered the collision")

        local player = msg.objectID

        -- Cinematic should only happen once - Cinema Once keeps it from getting more than one collision message, Tool-tip settings prevent it from ever re-occuring when player returns.
        if (bCINEMA_ONCE == false) then
           -- Update bool so that cinematic never plays again.
            bCINEMA_ONCE = true

            -- Disable player control and set a timer to return it
            --player:SetUserCtrlCompPause{bPaused = true}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 14.5 , "PauseTime", self )

            -- Play cinematic.
            player:PlayCinematic { pathName = "LandingCam" }

			player:DisplayZoneSummary{sender = self, isZoneStart = true}

        end

    end
end

