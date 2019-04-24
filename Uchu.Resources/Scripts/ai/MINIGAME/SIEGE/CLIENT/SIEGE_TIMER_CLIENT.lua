

function onNotifyClientObject(self, msg)
	
	if msg.name == "JoinTime" then
	
		if msg.param1 ~= 0 then
		
			
		
			local time =  SecondsToClock(msg.param1 )

			UI:SendMessage("SiegeJoin", {{"startTimer",  time }} )
			
			
		else
		
			UI:SendMessage("SiegeJoin", {{"startTimer",  " " }} )
		end	
	
	
	end

	if (msg.name == "RoundTime" and msg.paramStr == "Gate") then
		
		local time =  SecondsToClock(msg.param1)
		if (msg.param1 ~= 0) then
		
			if msg.param1 < 6 and msg.param1 > 0 then
			
				UI:SendMessage("SiegeText", {{"Text", tostring(msg.param1) }} )
				
			else
				UI:SendMessage("SiegeText", {{"Text", "Preparation Time "..time }} )
				
			end
			
		elseif msg.param1 == 0 then
		
			UI:SendMessage("SiegeUI_Attack", {{"siege_showhide", "hide"  }})
			UI:SendMessage("SiegeUI_Defend", {{"siege_showhide", "hide"  }})
			UI:SendMessage("SiegeText", {{"Text",  "reset" }} )
		end
	
	end


	if msg.name == "RoundTime" and msg.paramStr == "round" then
		local time =  SecondsToClock(msg.param1)
		if msg.param1 ~= 0 then
			UI:SendMessage("SiegeUI", {{"sgtime", time }} )
		else
			UI:SendMessage("SiegeUI", {{"sgtime",  " " }} )
		end
		
	end

	if msg.name == "ShowHUD" then

		UI:SendMessage("SiegeText", {{"UI", "hide" }} )
		UI:SendMessage("SiegeUI", {{"UI", "show"}} )
	end

end


function SecondsToClock(sSeconds)
	local nSeconds = sSeconds
	
		if nSeconds == 0 then
		
		
			return "00:00";
			else
			nHours = string.format("%02.f", math.floor(nSeconds/3600));
			nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
			nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
			return nMins..":"..nSecs
		end
end
