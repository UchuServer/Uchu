require('o_mis')



function onStartup(self, msg)
    --print "preloading"
    local Frisbeed = self:GetObjectsInGroup{ group = "Frisbee"}.objects

    for i = 1, table.maxn (Frisbeed) do
        -- Seagull for cinematic
        if Frisbeed[i]:GetLOT().objtemplate == 7667 then
            Frisbeed[i]:PreloadAnimation{animationID = "throw", respondObjID = self}
            -- Frisbee
        elseif Frisbeed[i]:GetLOT().objtemplate == 7668 then
            Frisbeed[i]:PreloadAnimation{animationID = "throw", respondObjID = self}
        -- Lion
        elseif Frisbeed[i]:GetLOT().objtemplate == 3995 then
            Frisbeed[i]:PreloadAnimation{animationID = "boomerang-cinematic", respondObjID = self}
        -- Coalessa
        elseif Frisbeed[i]:GetLOT().objtemplate == 3257 then
            Frisbeed[i]:PreloadAnimation{animationID = "boomerang-cinematic", respondObjID = self}
        -- Hiding the real seagull on client
        end
    end

end

function onCollisionPhantom(self, msg)

    local playerID = GAMEOBJ:GetLocalCharID()
	local bCINEMA_ONCE = false
    if (msg.objectID:GetID() == playerID) then

        local player = msg.objectID
        local tooltipMsg = player:GetTooltipFlag{ iToolTip = 1 }

        -- Cinematic should only happen once - Cinema Once keeps it from getting more than one collision message, Tool-tip settings prevent it from ever re-occuring when player returns.
        if ((bCINEMA_ONCE == false) and (tooltipMsg) and (tooltipMsg.bFlag == false)) then
           -- Update bool so that cinematic never plays again.
            bCINEMA_ONCE = true
			local Frishack = self:GetObjectsInGroup{ group = "Seagull", ignorespawners = true}.objects[1]
			Frishack:SetVisible{visible = false}
            -- give the player the flag to let them tame pets
            player:Help{ rerouteID = player, iHelpID = 1 }

            -- Disable player control and set a timer to return it
			UI:SendMessage( "pushGameState", {{"state", "cinematic" }} )
			
            -- Play cinematic
            player:PlayCinematic { pathName = "Intro_Cine" }
			GAMEOBJ:GetTimer():AddTimerWithCancel( LEVEL:GetCinematicInfo( "Intro_Cine" )+1, "Message6", self )
			player:PlayNDAudioEmitter{m_NDAudioEventGUID = '{02486966-a047-4587-89f5-634c60d6c41c}' } 

            -- Play animations on all four objects--seagull, frisbee, Coalessa, and lion
            local Frisbeed = self:GetObjectsInGroup{ group = "Frisbee"}.objects

            for i = 1, table.maxn (Frisbeed) do
                -- Seagull for cinematic
                if Frisbeed[i]:GetLOT().objtemplate == 7667 then
                    Frisbeed[i]:PlayAnimation{animationID = "throw"}
                    Frisbeed[i]:SetOffscreenAnimation{bAnimateOffscreen = true}
                    -- Frisbee
                elseif Frisbeed[i]:GetLOT().objtemplate == 7668 then
                    Frisbeed[i]:PlayAnimation{animationID = "throw"}
                    Frisbeed[i]:SetOffscreenAnimation{bAnimateOffscreen = true}
                -- Lion
                elseif Frisbeed[i]:GetLOT().objtemplate == 3995 then
                    Frisbeed[i]:PlayAnimation{animationID = "boomerang-cinematic"}
                    Frisbeed[i]:SetOffscreenAnimation{bAnimateOffscreen = true}
                -- Coalessa
                elseif Frisbeed[i]:GetLOT().objtemplate == 3257 then
                    Frisbeed[i]:PlayAnimation{animationID = "boomerang-cinematic"}
                    Frisbeed[i]:SetOffscreenAnimation{bAnimateOffscreen = true}
                -- Hiding the real seagull on client
                end
            end

        end
    end
end

function onTimerDone(self, msg)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

    if (msg.name == "Message6") then
        player:SetTooltipFlag{ iToolTip = 24 , bFlag = true }
		UI:SendMessage( "popGameState", {{"state", "cinematic"}} )
		local Frishack = self:GetObjectsInGroup{ group = "Seagull", ignorespawners = true}.objects[1]
		Frishack:SetVisible{visible = true}
    end

end
