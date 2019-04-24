
-- Plays a sparkle FX when anything touches this phantom physics object

function onCollisionPhantom(self, msg)
       local target = msg.objectID
       target:PlayFXEffect{effectID = 3671 , effectType = "cast" }
end 










