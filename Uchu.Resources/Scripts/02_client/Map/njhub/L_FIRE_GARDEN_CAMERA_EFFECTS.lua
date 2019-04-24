--------------------------------------------------------------
-- Client-side script for a camera fx in the fire garden area of NJ Monastery
-- 
-- created austin... 11/23/10
-- updated austin... 8/19/10 - removed leftover lines from CP script
--------------------------------------------------------------


-- We start our effect when we hit the collision phantom
function onCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		--print("onCollisionPhantom")
		LEVEL:AttachCameraParticles( "ng_ninjago/nin_camera_fire_trans/nin_camera_fire_trans", { x = 0, y = 0, z = 3 } )
	end
end

-- We disable our effect when we leave the collision phantom
function onOffCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetControlledID()
	if msg.objectID:GetID() == player:GetID() then
		--print("offCollisionPhantom")
		LEVEL:DetachCameraParticles( "ng_ninjago/nin_camera_fire_trans/nin_camera_fire_trans")
		
	end
end 