--------------------------------------------------------------
-- Client-side script for a LUT and camera fx in the Lower pit area of CP
-- 
-- created brandi... 11/23/10
-- updated brandi... 12/8/10 - added checks that should have been in the script ot start with
-- updated abeechler... 1/11/11 - removed camera particle sparkle effects while in the pit
--------------------------------------------------------------


-- We start our effect when we hit the collision phantom
function onCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		LEVEL:DetachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky")

		LEVEL:CLUTEffect( "aura_mar_underground_LUT_gamefile.dds", 0.2, 0.0, 1.0, false )
	end
end

-- We disable our effect when we leave the collision phantom
function onOffCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		LEVEL:AttachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky", { x = 0, y = 0, z = 150 } )
		
		LEVEL:CLUTEffect( "(none)", 0.0, 1.0, 0.0, false )
	end
end