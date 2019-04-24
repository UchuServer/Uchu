function onStartup (self)
print "whatever"

end

function onCollision (self, msg)
			  print "i collided"
local song = self: GetVar ("music")

	if song then
	SOUND: PlaySequence (song)
				print song
	end

end
