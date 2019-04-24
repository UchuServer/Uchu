
function onNotifyObjectStageChoicebuilds(self, msg)

	if msg.name == "ChoicebuildChanged" then
		getObjectByName( self, "Stage" ):NotifyObject{name = msg.name, param1 = msg.param1, ObjIDSender = msg.ObjIDSender}
		
	elseif msg.name == "ChoicebuildSmashed" then
		getObjectByName( self, "Stage" ):NotifyObject{name = msg.name, ObjIDSender = msg.ObjIDSender}
		
	end
end