all: password-grant password-grant.pdb decode-token decode-token.pdb authorization-code-flow authorization-code-flow.pdb

src/password-grant/bin/Debug/net6.0/linux-arm64/publish/password-grant: src/password-grant/*.cs src/password-grant/*.csproj
		cd src/password-grant ; dotnet publish

password-grant: src/password-grant/bin/Debug/net6.0/linux-arm64/publish/password-grant
		cp src/password-grant/bin/Debug/net6.0/linux-arm64/publish/password-grant password-grant

password-grant.pdb: src/password-grant/bin/Debug/net6.0/linux-arm64/publish/password-grant
		cp src/password-grant/bin/Debug/net6.0/linux-arm64/publish/password-grant.pdb password-grant.pdb

src/decode-token/bin/Debug/net6.0/linux-arm64/publish/decode-token: src/decode-token/*.cs src/decode-token/*.csproj
		cd src/decode-token ; dotnet publish

decode-token: src/decode-token/bin/Debug/net6.0/linux-arm64/publish/decode-token
		cp src/decode-token/bin/Debug/net6.0/linux-arm64/publish/decode-token decode-token

decode-token.pdb: src/decode-token/bin/Debug/net6.0/linux-arm64/publish/decode-token
		cp src/decode-token/bin/Debug/net6.0/linux-arm64/publish/decode-token.pdb decode-token.pdb

src/authorization-code-flow/bin/Debug/net6.0/linux-arm64/publish/authorization-code-flow: src/authorization-code-flow/*.cs src/authorization-code-flow/*.csproj
		cd src/authorization-code-flow ; dotnet build ; dotnet publish

authorization-code-flow: src/authorization-code-flow/bin/Debug/net6.0/linux-arm64/publish/authorization-code-flow
		cp src/authorization-code-flow/bin/Debug/net6.0/linux-arm64/publish/authorization-code-flow authorization-code-flow

authorization-code-flow.pdb: src/authorization-code-flow/bin/Debug/net6.0/linux-arm64/publish/authorization-code-flow
		cp src/authorization-code-flow/bin/Debug/net6.0/linux-arm64/publish/authorization-code-flow.pdb authorization-code-flow.pdb

clean:
	cd src/password-grant ; dotnet clean ; rm -r bin ; rm -r obj
	cd src/decode-token ; dotnet clean ; rm -r bin ; rm -r obj
	cd src/authorization-code-flow ; dotnet clean ; rm -r bin ; rm -r obj
	rm password-grant password-grant.pdb decode-token decode-token.pdb authorization-code-flow authorization-code-flow.pdb


