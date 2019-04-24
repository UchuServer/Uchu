--------------------------------------------------------------
-- Adds blizzard effects while in this volume.
-- created pml... 11/10/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
    LEVEL:AttachCameraParticles( "environment/frostburgh_blizzard/frostburgh_blizzard", { x = 0, y = 0, z = 3 } )
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	LEVEL:DetachCameraParticles( "environment/frostburgh_blizzard/frostburgh_blizzard" )
end 