function onCollisionPhantom(self, msg)

	print ("I teleported to the ship.")

    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PlayerTele",self )

end

function onTimerDone (self,msg)
	
	if (msg.name == "PlayerTele") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
            player:SetPosition {pos = {x=246,y=2642,z=-757}}
            player:SetRotation {x=0,y=0.628,z=0,w=0.777}
    end
    
end
