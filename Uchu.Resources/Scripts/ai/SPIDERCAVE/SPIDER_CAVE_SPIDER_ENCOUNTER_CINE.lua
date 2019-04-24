---------------------------------------------------------------------------
--tells the client to play a cinematic
---------------------------------------------------------------------------

function onCollisionPhantom(self, msg)
   self:NotifyClientObject{name = "playCine"}
end