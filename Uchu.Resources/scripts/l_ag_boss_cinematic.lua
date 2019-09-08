require('o_mis')
--//////////////////////////////////////////////////////////////////////////////////

local camName = "SpiderBoss_Cam_01" -- cinematic name
local camTime = 9 -- time to release player, length of cinematic
local enteringTrigger = false
--//////////////////////////////////////////////////////////////////////////////////

function onCollisionPhantom(self, msg)
    -- define player 313
    local playerID = GAMEOBJ:GetLocalCharID()

    -- exclusion checks
    if msg.objectID:GetID() ~= playerID then return end

    if enteringTrigger then
        enteringTrigger = false
        return
    end

    local player = msg.objectID
    
    enteringTrigger = true
    player:FlashNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Spider-Cave-Cinematic"}

    if msg.objectID:GetFlag{iFlagID = 34}.bFlag or msg.objectID:GetMissionState{missionID = 313}.missionState < 2 then return end

	GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "SpiderShow",self )

    player:SetFlag{iFlagID = 34, bFlag = true}
    player:PlayCinematic { pathName = camName}
end

function onTimerDone(self, msg)

	if msg.name == "SpiderShow" then
        --print "Play spider anim"
        local Spidery = self:GetObjectsInGroup{ group = "Cave"}.objects

        for i = 1, table.maxn (Spidery) do
            -- Seagull for cinematic
            if Spidery[i]:GetLOT().objtemplate == 6308 then
                Spidery[i]:PlayAnimation{animationID = "enter"}
            end
        end
    end

end
