# Use Custom Agent Plugin in VSCode

In this practice we use a custom Agent Plugin [my-dotnet-review](https://github.com/windperson-demo-devops/my-dotent-review) that combine both Custom Agent and Agent Skill to do AI Code Review:

## Install Custom Agent Plugin in VSCode

1. Checkout or downlaod this git repository's [**origin**](https://github.com/dotnet-mcp-lab/DotNetPrjSkillDemo/releases/tag/origin) git tag:
   ```sh
   git checkout tags/origin -b demo
   ```
2. Open Agent Customizations window by clicking the second left icon on VSCode Agent Chat UI top bar:  
   ![](./screen_shots/open_agent_customizations_window.png)  
3. Click the "+" on User, than type the git url: **** on top pop-up:  
   ![](./screen_shots/install_custom_agent_by_add_git_url.png)  
4. Then press Enter, click YES on pop-up confirm dialog:  
   ![](./screen_shots/install_custom_agent_by_add_git_url_confirm.png)  
5. Custom Agent plugin will be installed:  
   ![](./screen_shots/install_custom_agent_by_add_git_url_done.png)
   And you can click it to see the deteail information that is from README.md of git repo.
   ![](./screen_shots/custom_agent_by_add_git_url_details.png)

Then we can close the Agent Customizations window and return to VSCode main UI to work woth it.

## Use services (MCP Server, Custom Agent, Agent Skill) provided in Custom Agent Plugin

The Custom Agent plugin will have the MS Learn MCP server installed:  
![](./screen_shots/mcp_server_from_plugin.png)

So we can use it during Agent session:
![](./screen_shots/mcp_server_from_plugin_demo.png)


The **my-dotnet-review** custom agent should  
![](./screen_shots/custom_agent_from_plugin.png)

The **/my-dotnet-review:code-reviewer** Agent Skill can also be used:  
![](./screen_shots/agent_skill_rom_plugin.png)

So we can use those two togehter like this:
```sh
/my-dotnet-review:code-reviewer review AgenticSearchOpenAiResponseApi project
```
![](./screen_shots/use_both_custom_agnet_and_agnet_skill.png)

---

