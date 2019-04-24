require('o_mis')

local currentKeyPlaceIndex = 1
local keyspawns = {7903,7904,7905,7906,7907,7908,7909,7910,7911,7912,7913,7914,7915}
local currentKeyLoadIndex = 1
local CONSTANTS = {}
-- how much imagination is repeatedly drained while the organ is being played
CONSTANTS["ORGAN_IMAGINATION_COST"] = 2
-- how often imagination is drained while playing the organ
CONSTANTS["ORGAN_COST_FREQUENCY"] = 4.0

function onStartup(self)

    -- set max users to 1
    self:SetActivityParams{ modifyActivityActive=true, activityActive = true, maxUsers = 1, modifyMaxUsers = true }
    -- Add organ to group so keys can be ennumerated for deletion
    self:AddObjectToGroup{ group = "Organ" }
    -- Make sure organ is playing its idle animation
    self:PlayAnimation{ animationID = "key-up" }
    --Make sure key spawns are reset
    currentKeyLoadIndex = 1
   
   --Relative position/rotation data
    local oPos = { pos = "", rot = ""}
    local oDir = self:GetObjectDirectionVectors()
    oPos.pos = self:GetPosition().pos
    oPos.rot = self:GetRotation()

    -- Spawn keys on client in front of organ for individual clickability; didn't do the math work needed to make this relative; organ needs a rotation of 20.4 on the y for this to work
    -- Key G
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -2.369, y= oPos.pos.y + 2.424, z= oPos.pos.z - 0.692, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key GS
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -1.845, y= oPos.pos.y + 2.736 , z= oPos.pos.z - 1.243, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
        -- Key A
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -1.795, y= oPos.pos.y + 2.424 , z= oPos.pos.z - 0.478, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key AS
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -1.267, y= oPos.pos.y + 2.736 , z= oPos.pos.z - 1.028, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key B
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -1.219, y= oPos.pos.y + 2.424 , z= oPos.pos.z - 0.264, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key C
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -0.642, y= oPos.pos.y + 2.424 , z= oPos.pos.z - 0.049, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key CS
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -0.116, y= oPos.pos.y + 2.736 , z= oPos.pos.z - 0.6, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key D
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + -0.064, y= oPos.pos.y + 2.424 , z= oPos.pos.z - -0.166, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key DS
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + 0.462, y= oPos.pos.y + 2.736, z= oPos.pos.z - 0.385, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key E
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + 0.508, y= oPos.pos.y + 2.424 , z= oPos.pos.z - -0.378, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key F
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + 1.086, y= oPos.pos.y + 2.424 , z= oPos.pos.z - -0.594, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key FS
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + 1.61, y= oPos.pos.y + 2.736 , z= oPos.pos.z - -0.042, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
        currentKeyLoadIndex = currentKeyLoadIndex + 1
    -- Key G2
        RESMGR:LoadObject { objectTemplate = keyspawns[currentKeyLoadIndex], x= oPos.pos.x + 1.66, y= oPos.pos.y + 2.424 , z= oPos.pos.z - -0.807, rw= oPos.rot.w, rx= oPos.rot.x, ry= oPos.rot.y, rz = oPos.rot.z, owner = self }
    
end

function onUse(self, msg)

    local player = msg.user
    local imagination = player:GetImagination{}.imagination

    if IsPlayerInActivity(self, player) == false and imagination > 0 then
        -- Add player to activity
        self:AddActivityUser{ userID = player }
        -- Notify keys that they should made clickable even in camera mode
            local friends = self:GetObjectsInGroup{ group = "KeysForOrgan" }.objects

            for i = 1, table.maxn (friends) do 
                if friends[i] then
                    friends[i]:NotifyObject{name = "Clickable"}
                end
            end
        -- Check to see if anyone else is using the organ
        if self:ActivityUserExists{userID = player}.bExists then
            -- Make sure no one else can use organ if you do
            self:EnableActivity{bEnable = true, rerouteID = player}
            -- Store player
            storeObjectByName(self, "OrganUser", player)
            -- Start timers to drain imagination
            GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["ORGAN_COST_FREQUENCY"], "DecreaseImagination", self )
        end
    end

end

function DecreasePlayersImagination( self )

    -- Define player
    local player = getObjectByName(self,"OrganUser")
	-- Get how much imagination the player has now
	local OldAmount = player:GetImagination{}.imagination
	-- Subtract the cost of using the organ
	local NewAmount = OldAmount - 2

    if ( NewAmount <= 0 ) then
        -- If  Imagination is less than 0, set to 0
        NewAmount = 0
        -- Stop timer to decrease imagination and reset organ
		GAMEOBJ:GetTimer():CancelTimer("DecreaseImagination", self)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "organcomplete", self )	
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "organreset", self )	
    else
        -- If imagination is greater than 0, then start a new timer to drain imagination
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["ORGAN_COST_FREQUENCY"], "DecreaseImagination", self )	
    end

	-- Update their imagination
	if ( player ~= nil ) then
        player:SetImagination{ imagination = NewAmount }
	end
	
end

function onFireEventServerSide( self, msg )	

-- If the client detects a movement, cancel imagination drain and reset organ
	if ( msg.args == "PlayerMoved" ) then
		GAMEOBJ:GetTimer():CancelTimer("DecreaseImagination", self)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "organreset", self )
    end

end

function onTimerDone(self, msg)

    if ( msg.name == "DecreaseImagination" ) then
        DecreasePlayersImagination( self )

    elseif msg.name == "organreset" then
        -- Notify keys that they should made clickable even in camera mode
            local friends = self:GetObjectsInGroup{ group = "KeysForOrgan" }.objects

            for i = 1, table.maxn (friends) do 
                if friends[i] then
                    friends[i]:NotifyObject{name = "Unclickable"}
                end
            end
        -- Defining user player and removing them from activity
        local users = self:GetAllActivityUsers().objects
        for index,user in ipairs(users) do
            self:RemoveActivityUser{ userID = user }
            self:EnableActivity{bEnable = false, rerouteID = user}
        end

    end

end

----------------------------------------------------------------
-- Returns true/false if a player is in the activity
-- takes SELF and a PLAYER object
----------------------------------------------------------------
function IsPlayerInActivity(self, player)
    -- check if player is in activity
    local existMsg = self:ActivityUserExists{ userID = player }
    
    if (existMsg) then
        return existMsg.bExists
    end
    return false
end