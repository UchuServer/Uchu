function onStartup(self)
    local oPos = self:GetPosition().pos
    print("********** Warning **********")
    print("Hello my name is: " .. self:GetName().name)
    print("I'm located at: x = " .. oPos.x .. " y = " .. oPos.y .. " z = " .. oPos.z)
    print("Please remove L_ACT_DEAD_WATER_TEAM_4.lua from my client side script in HF.")
    print("Thank You")
    print("*****************************")
end