-- ================================================
-- SG Server Zone Script
-- Server Side
-- updated 9/30/10 mrb... updated player loading
-- ================================================

local m_totalscore = 0

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)
    self:SetVar("CONSTANTS", CONSTANTS)
	self:SetVar("timelimit", waves[1].timeLimit)
end

--------------------------------------------------------------
-- put the player in the cannon, does not start the cannon
--------------------------------------------------------------
function enterCannon(self)
    if self:GetVar("bLoaded") then return end
    
	-- get the cannon
	local cannon = getObjectByName("cannonObject")
	-- get the player
	local player = getObjectByName("activityPlayer")
    
    if not cannon or not player then return end
    
	-- if we have both start it
	if ((cannon:Exists()) and (player:Exists())) then
        freezePlayer(player, true)    
		self:SetVar("bLoaded", true)
		cannon:RequestActivityEnter{bStart = false, userID = player}
	end
end

function freezePlayer(playerID, bFreeze)
    local eChangeType = "POP"
    local bVisible = true
    
    if bFreeze then    
        eChangeType = "PUSH"
        bVisible = false
    end
        
    playerID:SetStunned{ StateChangeType =  eChangeType,
                                            bCantMove = true,
                                            bCantAttack = true,
                                            bCantInteract = true }    
end

--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function mainPlayerLoaded(self, msg)
	----print ("Player Entered: " .. msg.playerID:GetName().name)

	-- stun and move player to level start location
	-- @TODO: Sometimes this teleport works and sometimes it does not.....ghosting distance?
	local player = msg.playerID

	player:CancelMission{ missionID = 30 }
    storeObjectByName("activityPlayer", msg.playerID)
end

--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function mainObjectLoaded(self, msg)
	-- Cannon Object Loaded
	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) then
        -- store the cannon object for use later
        storeObjectByName("cannonObject", msg.objectID)  
        -- try to put hte player in the cannon
        enterCannon(self)
	end
end

function onActivityStateChangeRequest(self,msg)
	if (msg.wsStringValue == 'clientready') then
		-- put the player in the cannon
		enterCannon(self)
	end
end 