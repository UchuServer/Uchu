
---------------------------------------------------------------------------------------
--  Sent to the racing control object when a player's rank in the race has changed.
---------------------------------------------------------------------------------------
function onRacingPlayerRankChanged(self, msg) 
	
		local playerName = msg.playerID:GetName().name
		local player = msg.playerID
		local oldRank = msg.oldRank
		local newRank = msg.newRank	

	
	if self:GetVar("RaceStarted") then
	
		--print("onRacingPlayerRankChanged "..oldRank.."   "..playerName)	
		UI:SendMessage("RaceStat", {{"racePos_"..oldRank, playerName }} )
		
	end
	
end
---------------------------------------------------------------------------------------
--  Sent to the racing control object when a player's 'wrong-way' status changes.
---------------------------------------------------------------------------------------
function onRacingPlayerWrongWayStatusChanged(self, msg)

	local player = msg.playerID
	local way = msg.bGoingWrongWay
	if (way) then
		UI:SendMessage("RaceWW", {{"UI","show" }} )
	else
		UI:SendMessage("RaceWW", {{"UI","hide" }} )
	end
	
end

---------------------------------------------------------------------------------------
--   Lets racing control object know when a player has left the track's 
---------------------------------------------------------------------------------------
function onRacingPlayerOutOfTrackBounds(self, msg)

	local player = msg.playerID
	local carID = getObjectByName(self, player:GetName().name.."CarID")

	if (carID:VehicleCanWreck().bCanWreck == true) then

		carID:RequestDie{ killType = "VIOLENT" }
		
	end
	
end
