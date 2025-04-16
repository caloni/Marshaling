/* SPDX-License-Identifier: GPL-2.0
 *
 * Copyright (C) 2018-2021 WireGuard LLC. All Rights Reserved.
 */

#include "pch.h"
#include "wireguard.h"
#include <Windows.h>
#include <stdlib.h>

WIREGUARD_SET_CONFIGURATION_FUNC WireGuardSetConfiguration;
_Use_decl_annotations_
BOOL WINAPI
WireGuardSetConfiguration(WIREGUARD_ADAPTER_HANDLE *Adapter, const WIREGUARD_INTERFACE *Config, DWORD Bytes)
{
    const WIREGUARD_PEER* Peer = (const WIREGUARD_PEER*) &Config[1];
    const WIREGUARD_ALLOWED_IP* AllowedIPs = (const WIREGUARD_ALLOWED_IP*) &Peer[1];
    (Peer);
    (AllowedIPs);
    return TRUE;
}

