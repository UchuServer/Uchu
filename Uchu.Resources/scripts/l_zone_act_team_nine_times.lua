-- Shamelessly stolen from L_ZONE_ACT_SHOOTING_GALLERY_1.lua

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}

-- Cannon constants
CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["CANNONREBUILD_TEMPLATEID"] = 2781
CONSTANTS["CANNON_PLAYER_OFFSET"] = {x = 6.652, y = 0, z = -5.716}
CONSTANTS["CANNON_VELOCITY"] = 175.0
CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 1500.0
CONSTANTS["CANNON_BARREL_OFFSET"] = {x = 0, y = 0, z = 0}
CONSTANTS["CANNON_TIMEOUT"] = 60.0

--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

	-- Cannon rebuild loaded
	-- For some reason the platform rebuild doesn't break if we tell it to here?
	if (msg.templateID == CONSTANTS["CANNONREBUILD_TEMPLATEID"]) then
 	    msg.objectID:RebuildReset()
    end

	-- Cannon Object Loaded
	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) or (msg.templateID == CONSTANTS["CANNONREBUILD_TEMPLATEID"]) then
		-- Override the cannon shooting parameters
		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["CANNON_PLAYER_OFFSET"],
		                                      projectileVelocity = CONSTANTS["CANNON_VELOCITY"],
		                                      cooldown =           CONSTANTS["CANNON_REFIRE_RATE"],
		                                      muzzlePosOffset =    CONSTANTS["CANNON_BARREL_OFFSET"],
		                                      minDistance =        CONSTANTS["CANNON_MIN_DISTANCE"],
		                                      timeLimit =          CONSTANTS["CANNON_TIMEOUT"]}
	end

end
