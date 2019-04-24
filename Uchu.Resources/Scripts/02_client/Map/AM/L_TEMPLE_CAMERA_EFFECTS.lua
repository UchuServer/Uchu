--------------------------------------------------------------
-- Client-side script for a camera fx in the temple area of CP
-- 
-- created brandi... 11/23/10
-- updated brandi... 12/8/10 - added checks that should have been in the script ot start with
--------------------------------------------------------------


-- We start our effect when we hit the collision phantom
function onCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		--print("onCollisionPhantom")
		LEVEL:DetachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky")
		LEVEL:AttachCameraParticles( "auramar/environment/am_ninjago_camera/am_ninjago_camera", { x = 0, y = 0, z = 3 } )
	end
end

-- We disable our effect when we leave the collision phantom
function onOffCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		--print("offCollisionPhantom")
		LEVEL:DetachCameraParticles( "auramar/environment/am_ninjago_camera/am_ninjago_camera")
		LEVEL:AttachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky", { x = 0, y = 0, z = 150 } )
	end
end 