function onStartup(self)
	self:SetVar("IconNumber", 1) 
	self:SetVar("MyPriority", 0.8) 
end

function onGetPriorityPickListType(self, msg)
    local priority = self:GetVar("MyPriority") or 0
    
	if ( priority > msg.fCurrentPickTypePriority ) then
		msg.fCurrentPickTypePriority = self:GetVar("MyPriority")
		msg.ePickType = 14    -- Interactive pick type
	else
		msg.ePickType = -1
	end
	
	return msg
end

function onNotifyClientObject(self, msg)
	
	if (msg.name == "updateCarNumber" and msg.param1 > 0) then
		if self:GetVar("IconNumber") ~= msg.param1 then
			--	print("SetIcon OFF")
			self:SetIconAboveHead{iconType = 400, bIconOff = true, overrideIsClickable = false }
			--	print("Set custom icon")
			self:SetIconAboveHead{iconType = 400, overrideIconLOT = msg.param1, overrideIsClickable = false }
			self:SetVar("IconNumber", msg.param1 ) 		
		end
	--else
		--print("SetIcon OFF")
		self:SetIconAboveHead{iconType = 400, bIconOff = true, overrideIsClickable = false }
	end
end

