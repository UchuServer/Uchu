--------------------------------------------------------------
-- Client zone script for lup station, plays a cinematic the players first time to the station
-- 
-- created by brandi... 11/22/10
--------------------------------------------------------------

----------------------------------------------
-- when the player is ready, start thecientmatic
----------------------------------------------
function onPlayerReady(self,msg)
	-- get the local player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	-- player the intro cinematic
	player:PlayCinematic { pathName = "IntroCam_3" }
end