
function onCollision(self, msg)
	        self:PlayFXEffect{effectType = "trail"}
		--print "i just cast a skill"
            self:KillObj{targetID = self}
		--print "i just killed myself"
	    
	end       	
	return msg
end