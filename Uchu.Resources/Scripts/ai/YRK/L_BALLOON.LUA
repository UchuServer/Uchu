

require('o_mis')
require('c_Zorillo')


CONSTANTS["SUFFICIENT_STINK"] = 2			-- how much stink it takes to launch the balloon.  Usually, each spot with a stinky player counts as 1.

CONSTANTS["SKUNK_STINK_SKILL"] = 33			-- if a player has this skill, they have skunk stink on them
CONSTANTS["IMITATION_STINK_SKILL"] = 35		-- if a player has this skill, they have imitation stink on them ( from something like perfume )

CONSTANTS["RECHECK_TIME"] = 0.5				-- if there's anyone on the balloon, how often to check for launch




--------------------------------------------------------------
-- On startup
--------------------------------------------------------------
function onStartup( self )

	self:StopPathing{}				-- start out stationary on the ground
	
	ResetStink( self )
	
	self:SetVar( "bBalloonInUse", false )		-- whether the balloon is currently moving
	
	self:SetVar( "bPlayerOnBalloon", false )	-- whether there is at least one player on the balloon ( stinky or not )

	registerWithZoneControlObject(self)
end




--------------------------------------------------------------
-- the trigger was just stepped on
--------------------------------------------------------------
function onFireEvent( self, msg )

	--print ( "inventor balloon onFireEvent" )
	
	-- remember each trigger's ID as we get it, so that when any trigger is touched, we can check all 4
	if ( msg.args == "switchOn" ) then
		storeObjectByName( self, "triggerID", msg.senderID )
	end
	
	
	-- we don't care who gets on or off the triggers while the balloon is up in the air
	if ( self:GetVar( "bBalloonInUse" ) == true ) then
		return
	end	
		

	-- check the stink level at each of the 4 switches and see if we have enough total to launch the balloon
	CheckForLaunch( self )
	
end




--------------------------------------------------------------
-- timer done
--------------------------------------------------------------
function onTimerDone( self, msg )
	
    if (msg.name == "recheckTimer") then
		--print( "recheckTimer" )
        CheckForLaunch( self )
       
	
	elseif (msg.name == "CreditCheckTimer") then
       GiveCreditForLaunching( self )
	end
    
end




--------------------------------------------------------------
-- reset the currentStink variable
--------------------------------------------------------------
function ResetStink( self )
	self:SetVar( "currentStink", 0 )
end




--------------------------------------------------------------
-- set the currentStink variable to the amount needed to launch the balloon
--------------------------------------------------------------
function SetStinkToMax( self )
	self:SetVar( "currentStink", CONSTANTS["SUFFICIENT_STINK"] )
end




--------------------------------------------------------------
-- see if there are enough stinky players on the trigger to launch the balloon
--------------------------------------------------------------
function CheckForLaunch( self )

	--print( "CheckForLaunch" )
	
	-- remember how much stink was fueling the balloon before in case we need to change the anim
	local previousStink = self:GetVar( "currentStink" )
	

	ResetStink( self )
	self:SetVar( "bPlayerOnBalloon", false )

	-- check how much stink is on players at the switch and store that in "currentStink"
	UpdateStinkAtSwitch( self )
	
	--print( "CheckForLaunch: stink amount is " .. self:GetVar( "currentStink" ) )
	
	
	-- if the amount of stink changed,
	-- change the balloon's animation so it matches how much stink is fueling it
	local newStink = self:GetVar( "currentStink" )
	if ( newStink ~= previousStink ) then
		UpdateAnimation( self, newStink )
	end
	
	
	if ( newStink >= CONSTANTS["SUFFICIENT_STINK"] ) then
		LaunchBalloon( self )
		
	else
		--  as long as there is even one player on the balloon,
			--we need to keep checking the amount of stink even if noone new gets on
			-- in case a player uses the skunk perfume while already on the ballon
		if ( self:GetVar( "bPlayerOnBalloon" ) == true ) then
			SetRecheckTimer( self )
		end
	end
end





--------------------------------------------------------------
-- there is enough stink, so start moving the moving platform
--------------------------------------------------------------
function LaunchBalloon( self )

	--print ( "inventor balloon, LAUNCH BALLOON!" )
	
	self:SetVar( "bBalloonInUse", true )
	
	self:StartPathing{}						-- send the balloon up
	
	-- give any stinky players on the balloon credit towards an achievement
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "CreditCheckTimer", self )
		
end





--------------------------------------------------------------
-- check how much stink the player's on the switch have
--------------------------------------------------------------
function UpdateStinkAtSwitch( self )

	-- check how much stink is on players at the switch and store that in "currentStink"

	local triggerID = nil
	triggerID = getObjectByName( self, "triggerID" )
	
	if ( triggerID == nil ) then
		return
	end


	-- look for the players near the trigger
	local objs = triggerID:GetProximityObjects().objects
	
	local stinkAmount = 0
	
	local index = 1
	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		if ( faction and faction.faction == 1 ) then	-- it's a player
		
			--print( "player on switch" )
			
			-- remember there's someone on the balloon
			-- it doesn't matter if they're stinky or not.  We'll need to keep checking in case they use stink perfume while on there
			self:SetVar( "bPlayerOnBalloon", true )
			
		
			-- check if the player has imitation stink ( from something like perfume )
			-- if any one player has perfume stink, they are considered stinky enough to launch the balloon alone
			if ( target:IsSkillActive{ iSkillID = CONSTANTS["IMITATION_STINK_SKILL"] }.bOn ) then
			
				--print( "found a player with perfume" )
			
				SetStinkToMax( self )
				return
			end
			
			
			-- check if the player has skunk stink
			-- skunk stink counts as one ration of stinkiness towards fueling the balloon
			if ( target:IsSkillActive{ iSkillID = CONSTANTS["SKUNK_STINK_SKILL"] }.bOn ) then
	
				--print( "found a player with skunk stink" )
				
				stinkAmount = stinkAmount + 1
				if ( stinkAmount >= CONSTANTS["SUFFICIENT_STINK"] ) then
					SetStinkToMax( self )
					return
				end
			end
			
		end
		
		index = index + 1
	end
	
	
	self:SetVar( "currentStink", stinkAmount )
end




--------------------------------------------------------------
-- see the timer to check the stink amount again
--------------------------------------------------------------
function SetRecheckTimer( self )
	GAMEOBJ:GetTimer():CancelTimer("recheckTimer", self)
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["RECHECK_TIME"], "recheckTimer", self )
end




--------------------------------------------------------------
-- give achievement credit for every stinky player on the switch when the balloon launches
--------------------------------------------------------------
function GiveCreditForLaunching( self )

	-- anyone who's on the balloon and is stinky when it launches gets credit for helping
	-- this is towards an achievement
	
	triggerID = getObjectByName( self, "triggerID" )

	
	-- look for the players near the trigger
	local objs = triggerID:GetProximityObjects().objects
	
	local index = 1
	while index <= table.getn(objs)  do
	
		local skunkStink = false
		local imitationStink = false
	
		local target = objs[index]
		local faction = target:GetFaction()
		
	
		
		if ( faction and faction.faction == 1 ) then	-- it's a player
	
			-- check if the player has skunk stink	
			if ( target:IsSkillActive{ iSkillID = CONSTANTS["SKUNK_STINK_SKILL"] }.bOn ) then
			
				skunkStink = true
			end
			
			-- check if the player has imitation stink ( from something like perfume )
			if ( target:IsSkillActive{ iSkillID = CONSTANTS["IMITATION_STINK_SKILL"] }.bOn ) then
	
				imitationStink = true
			end
			
			-- if the player has either type of stink, give them credit
			if ( skunkStink or imitationStink ) then
				
				--achievement info here
				--this is hacky and not the right way we should be doing this, but it works, non the less
				-- Using the "Kill" mission type in the DB, we are able to update the mission through the objects by calling
				--the "kill" tasktype with calues of one.  The downfall is that this will only allow one mission on the designated objects that
				--use the kill type.  The commented out updatemissiontask functions are the way we should be doing this but is broken.
				target:UpdateMissionTask {target = self, value = 1, value2 = 1, taskType = "kill"}
				--target:UpdateMissionTask{target = self, value = 161, value2 = 1, taskType = "complete" }
				--target:UpdateMissionTask{target = self, value = 162, value2 = 1, taskType = "complete" }
				--target:UpdateMissionTask{target = self, value = 163, value2 = 1, taskType = "complete" }
				
				--print( "balloon achievement updated" )
			end
			
		end
		
		index = index + 1
	end
	
end




--------------------------------------------------------------
-- change the animantion to reflect how much stink is currently feeding into it
--------------------------------------------------------------
function UpdateAnimation( self, stinkAmount )
	
	-- return early if render is not ready
    if ( self:GetVar( "bRenderReady" ) == false ) then
        return
    end
    

	if ( stinkAmount == 0 ) then
		self:PlayAnimation{ animationID = "balloon1" }
	
	elseif ( stinkAmount == 1 ) then
		self:PlayAnimation{ animationID = "balloon2" }
	
	elseif ( stinkAmount == CONSTANTS["SUFFICIENT_STINK"] ) then
		self:PlayAnimation{ animationID = "balloon3" }
		
	end
end





--------------------------------------------------------------
-- Notification to object
--------------------------------------------------------------
function onNotifyObject( self, msg )

	if ( msg.name == "playerLoaded" ) then
	
		-- tell the client-side script for the just-loaded player to use the appropriate anim based on how inflated the balloon is
		self:NotifyClientRebuildSectionState{ rerouteID = msg.ObjIDSender, iState = self:GetVar( "currentStink" ) }
	end
end



function onArrived( self, msg )

	--print( "BALLOON onArrived" )
	--print( "arrived at waypoint: ")
	--print( msg.wayPoint)
	
	if( msg.wayPoint == CONSTANTS["LAST_BALLOON_WAYPOINT"] ) then
	
		--print( "at last waypoint" )
		
		self:StopPathing{}
		
		self:SetVar( "bBalloonInUse", false )
		
		CheckForLaunch( self )							-- check whether enough stinky players stayed on the platform to launch it again
	end

end


