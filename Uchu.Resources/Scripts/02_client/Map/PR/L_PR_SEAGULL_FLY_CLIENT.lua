--------------------------------------------------------------
-- Client side script on the seagull in Pet Cove
-- when the server script tells this script to, this script animates the seagull

-- created by Brandi... 3/10/11
--------------------------------------------------------------

----------------------------------------------
-- the bird has loaded for the client
----------------------------------------------
function onRenderComponentReady(self,msg)
	-- if the bird is supposed to be in the air, start the animation
	if not self:GetNetworkVar("BirdLanded") then
		BirdUp(self)
	end
end

----------------------------------------------
-- messages sent from the client
----------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
	-- parse through the table of network vars that were updated
    for k,v in pairs(msg.tableOfVars) do
		
        -- the bird landed, plan the animation
        if k == "BirdLanded" and v then
			BirdDown(self)
		-- the bird took off, play the fly animation
		elseif k == "BirdLanded" and not v then
			BirdUp(self)
		end
	end
end

----------------------------------------------
-- timer done: play hover animation
----------------------------------------------
function onTimerDone(self, msg)
    if (msg.name == "flyaway") then
		-- the fly animation is done, play the hover animation
		local seagull = self:GetObjectsInGroup{ group = "Seagull", ignoreSpawners = true}.objects[1]
		if not seagull then return end
		seagull:PlayAnimation{animationID = "fly-hover"}
	end
end

----------------------------------------------
-- Custom function: play fly animation
----------------------------------------------
function BirdUp(self)
	-- play the fly away animation and start a timer to play the hover animation
	local seagull = self:GetObjectsInGroup{ group = "Seagull", ignoreSpawners = true}.objects[1]
	if not seagull then return end
	seagull:PlayAnimation{animationID = "fly-away"}
	GAMEOBJ:GetTimer():AddTimerWithCancel( seagull:GetAnimationTime{  animationID = "fly-away" }.time , "flyaway", self )
end

----------------------------------------------
-- Custom function: play land animation
----------------------------------------------
function BirdDown(self)
	-- play the land animation
	local seagull = self:GetObjectsInGroup{ group = "Seagull", ignoreSpawners = true}.objects[1]
	if not seagull then return end
	seagull:PlayAnimation{animationID = "fly-land"}
end