
function onStartup(self)



end





function onNotifyClientObject(self, msg)

	if msg.name == "RoundTime" then
		UI:SendMessage("SiegeText", {{"Text", "Preparation Time "}} )

		UI:SendMessage("SiegeUI", {{"sgtime",  tostring(msg.parama1) }} )
				local mySeconds = string.format("%2f", secs-(Math.Floor(secs/60)*60);msg.param1);
			print(mySeconds)
	end
	
	if msg.name == "ShowHUD" then
		print("ShowHUD *******************************")
		UI:SendMessage("SiegeText", {{"UI", "hide" }} )
		UI:SendMessage("SiegeUI", {{"UI", "show"}} )
	end


end