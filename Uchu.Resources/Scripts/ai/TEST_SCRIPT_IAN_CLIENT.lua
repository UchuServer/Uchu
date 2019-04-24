local soundID
function onOnHit(self, msg)
	--self:DisplayChatBubble{wsText = "Punches to the face! My one weakness!"};
	--soundID = SOUND:StopSequence3D(soundID);
	--SOUND:StopSequence(soundID);
end

function onStartup(self)
--print("on startup")
--	print("should be tacos")
--	print(self:GetNetworkVar("tableVar1"))
	
--	print("should be nil")
	--self.PlayAnimation{ animationID = "idle" }
	--self:PlayAnimation{ animationID = "idle" }
	print("I'm alive!")
	
end

function onScriptNetworkVarUpdate(self,msg)
print("Variables changed!")

--print (type(msg.tableOfVars[1][1]))
--print (tablemsg.tableOfVars[1])
--print type(msg.tableOfVars[1][1])

	local i = 1
	while i <= #msg.tableOfVars do
		print(msg.tableOfVars[i][1])
		print(msg.tableOfVars[i][2])
		i = i + 1
	end
	
	if(msg.tableOfVars[1][2] == true ) then
		print("first var was true!")
	end
end
