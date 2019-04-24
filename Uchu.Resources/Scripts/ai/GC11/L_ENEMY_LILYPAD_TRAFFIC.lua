-- require('State')
--require('o_StateCreate')
--require('o_mis')
--require('o_Main')
function onStartup(self) 
	Set = {}
	self:FollowWaypoints()
	Set['WanderSpeed']       = 50          -- Move speed 
end

-- called on every waypoint (bug with final waypoint
function onArrived(self, msg)
	print("onArrived", msg.wayPoint)
	
    if msg.actions then
	--print("some actions")
	--print(msg.actions[1].name)
	if msg.actions and msg.actions[1].name == "onAlmostEnd" then
		print("yep im dead")
		self:KillObj{targetID=self}
	end
    end
    --self:ContinueWaypoints();  -- Explained below
end

