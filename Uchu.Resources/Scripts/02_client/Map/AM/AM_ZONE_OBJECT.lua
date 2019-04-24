--------------------------------------------------------------
-- Client Zone script for Aura Mar

-- created brandi... 11/8/10 
-- updated brandi... 11/23/10 - changed to onPlayerReady function
--------------------------------------------------------------

function onPlayerReady(self, msg)
	LEVEL:DetachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky")
	-- play aura mar camera effect
	LEVEL:AttachCameraParticles( "auramar/environment/aura_mar_sky/aura_mar_sky", { x = 0, y = 0, z = 150 } )--prototype/snow/snow
end
