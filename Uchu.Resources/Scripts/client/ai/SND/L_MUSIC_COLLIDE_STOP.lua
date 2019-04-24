function onCollision(self,msg)
	local song = self:GetVar("music");	
	
		if(song) then
		SOUND:StopSequence(song);
		self: SetVar ("music", nil)
	end

	msg.ignoreCollision = true;
	return msg

end

function onNotifyObject (self, msg)
   self: SetVar ("music", msg.param1)
   

   
end
