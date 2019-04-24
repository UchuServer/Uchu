-----------------------------------------------------------------------------
--client script to play effects on the crypt FX object
-----------------------------------------------------------------------------

function onRenderComponentReady(self) 

   self:FireEventServerSide{senderID = self, args = "DoorReady"}
end