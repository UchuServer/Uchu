




--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('c_AvantGardens')





--------------------------------------------------------------
-- constants
--------------------------------------------------------------
CONSTANTS["DISCO_RECHECK_DELAY"] = 1.0
CONSTANTS["DISCO_RADIUS"] = 30



--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )

	self:SetProximityRadius{ radius = CONSTANTS["DISCO_RADIUS"] }
	
	SetTimerToCheckProximity( self )
	
end






--------------------------------------------------------------
-- Set a timer to check whether the player is dancing nearby
--------------------------------------------------------------
function SetTimerToCheckProximity( self )

	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["DISCO_RECHECK_DELAY"], "checkProximity", self )
	
end






--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone( self, msg )
	
	if ( msg.name == "checkProximity" ) then
		
		CheckProximity( self )
		SetTimerToCheckProximity( self )
	
	end
	
end





--------------------------------------------------------------
-- check whether the local player is nearby doing a dance emote
-- if so, make the crowd do the same dance
-- for each item in this NPC's proximity:
	-- first check if it's the local player
	-- and is playing the corresponding anim
--------------------------------------------------------------
function CheckProximity( self )

	-- get a list of all the objects within proximity
	local proxObjs = self:GetProximityObjects().objects
	

	-- go through the list and check each one
	for i = 1 , table.maxn( proxObjs ) do

		local nearbyObj = proxObjs[i]
		
		-- check to see if it's the local player and doing a dance emote
		local emoteNum = GetLocalPlayersDanceEmote( nearbyObj )
		if ( emoteNum ~= -1 ) then
		
			MakeCrowdDance( self, emoteNum )
			return
			
		end

	end
	
	MakeCrowdStop( self )
	
end





--------------------------------------------------------------
-- returns the name of the dance emote ID that the local player is playing
-- or -1 if the  local player isn't playing a dance emote nearby
--------------------------------------------------------------
function GetLocalPlayersDanceEmote( obj )

	-- check if the object is even a player
	if ( IsLocalPlayer( obj ) == false ) then
		return -1
	end
	
	
	for emoteIndex = 1 , table.maxn( CONSTANTS["DISCO_ANIMS"] ) do

		if ( IsPlayingEmoteAnim( obj, CONSTANTS["DISCO_ANIMS"][emoteIndex] ) ) then

			return CONSTANTS["DISCO_ANIMS"][emoteIndex]
			
		end
		
	end
	

	return -1		
		
end




--------------------------------------------------------------
-- returns whether or not given the obj is the local character
--------------------------------------------------------------
function IsLocalPlayer( obj )

	return ( obj:GetID() == GAMEOBJ:GetLocalCharID() )
	
end






--------------------------------------------------------------
-- returns whether or not the given player is playing the given anim
--------------------------------------------------------------
function IsPlayingEmoteAnim( player, anim )
	
	local emoteMsg = player:GetCurrentAnimation{}
	return ( emoteMsg.primaryAnimationID == anim or 
			emoteMsg.secondaryAnimationID == anim )
	
end




--------------------------------------------------------------
-- any NPC's in the group should play the given anim
--------------------------------------------------------------
function MakeCrowdDance( self, anim )

	local crowdObjects = self:GetObjectsInGroup{ group = CONSTANTS["CONCERT_FAN_GROUP"] }.objects

	for i = 1, table.maxn ( crowdObjects ) do
	
		crowdObjects[i]:PlayAnimation{ animationID = anim }
	end
		
end





--------------------------------------------------------------
-- any NPC's in the group should play their idles
--------------------------------------------------------------
function MakeCrowdStop( self )

	local crowdObjects = self:GetObjectsInGroup{ group = CONSTANTS["CONCERT_FAN_GROUP"] }.objects

	for i = 1, table.maxn ( crowdObjects ) do
	
		crowdObjects[i]:PlayAnimation{ animationID = "idle" }
	end
		
end


