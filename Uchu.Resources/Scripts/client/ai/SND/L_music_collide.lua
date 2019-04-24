function onCollision(self,msg)
	local song = self:GetVar("music");
	if(song) then
		local soundID = SOUND:PlaySequence(song);

		local objects = self:GetObjectsInGroup {group = self:GetVar("group name")}.objects

		objects[1]:NotifyObject{param1=soundID}
	end

	msg.ignoreCollision = true;
	return msg
end
