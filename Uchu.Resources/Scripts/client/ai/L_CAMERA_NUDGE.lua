function onCollisionPhantom(self, msg)

    --print("IN VOLUME")

    CAMERA:SetDesiredPitchMax("CAMERA_MAIN_GAME_D", 160)

    CAMERA:SetDesiredPitchMin("CAMERA_MAIN_GAME_D", 125)

end

 

function onOffCollisionPhantom(self, msg)


    --print("OUT OF VOLUME")

    CAMERA:SetDesiredPitchMax("CAMERA_MAIN_GAME_D", 105)

    CAMERA:SetDesiredPitchMin("CAMERA_MAIN_GAME_D", 100)
 

end
