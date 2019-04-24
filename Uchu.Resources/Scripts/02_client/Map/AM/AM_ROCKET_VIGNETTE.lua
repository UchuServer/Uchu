--[[
-Script for playing a looping animation and shaking the camera for the player if he's within a certain distance.
-
--]]
local FXEffectID = 6030
local FXEffectType = "shake"
local FXTime = 3.33
local FXRadius = 300
local animationName = "explode"

function onRenderComponentReady(self, msg)
	self:PlayAnimation{animationID = animationName, bTriggerOnCompleteMsg = true}
    GAMEOBJ:GetTimer():AddTimerWithCancel( FXTime, "playFX", self )
end

function onAnimationComplete(self, msg)
	if msg.animationID == animationName then
		--animation is complete, restart it.
		self:PlayAnimation{animationID = animationName, bTriggerOnCompleteMsg = true, bPlayImmediate = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel( FXTime, "playFX", self )
	end
end

function onTimerDone(self, msg)
	if msg.name == "playFX" then
		local player = GAMEOBJ:GetControlledID()
		local distance = getDistance(player:GetPosition().pos, self:GetPosition().pos)
		--print ("distance: " .. distance)
		if distance < FXRadius then
			player:PlayFXEffect{effectID = FXEffectID, effectType = FXEffectType}
		end
	end
end

--Calculates the distance between two points
function getDistance(pos1, pos2)
	local vector = {}
	--find the vector that describes the line between the two points
	vector.x = pos1.x - pos2.x
	vector.y = pos1.y - pos2.y
	vector.z = pos1.z - pos2.z
	
	--length of a vector is L^2 = X^2 + Y^2 + Z^2
	return math.sqrt((vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z))
end