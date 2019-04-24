local canFire = true

function onCollision(self, msg)

    if canFire == true then
        
	print ("I hit the rocket")

    canFire = false

GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PlayerAnimate",self )
GAMEOBJ:GetTimer():AddTimerWithCancel( 9.9, "PlayerTeleport",self )

    end

end

function onTimerDone (self,msg)
	
	if (msg.name == "PlayerAnimate") then
	  self:PlayAnimation{animationID = "launch"}
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
            player:SetPosition {pos = {x=328.24,y=2627.17,z=-736.16}}
            player:SetRotation {x=0,y=0.628,z=0,w=0.777}
            player:SetUserCtrlCompPause{bPaused = true}
            player:PlayCinematic { pathName = "Rocket_Path_01" }
            player:PlayAnimation{animationID = "rocket"}
            
	elseif (msg.name == "PlayerTeleport") then
 	 local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:SetUserCtrlCompPause{bPaused = false}
		player:SetRotation {x=0,y=-0,z=0,w=1}
		player:SetPosition {pos = {x=200.31,y=759.37,z=-1162.71}}
        canFire = true
    end
    
end