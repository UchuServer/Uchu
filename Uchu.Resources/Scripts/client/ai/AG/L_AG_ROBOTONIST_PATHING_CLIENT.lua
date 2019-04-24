

function onNotifyObject( self, msg )

	if ( msg.name == "zonePlayer" ) then
		
		-- the mosaic portal needs to zone the player, but it doesn't exist server-side.  asking this object to do it instead.
		
		self:FireEventServerSide{ senderID = msg.ObjIDSender, args = "zonePlayer" }
	end
end