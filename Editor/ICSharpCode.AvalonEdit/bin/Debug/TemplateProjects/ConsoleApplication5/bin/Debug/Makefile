# This file is part of MXE.
# See index.html for further information.




LIBRARY_PREFIX :=
LIBRARY_SUFFIX :=

SED := $(shell gsed --help >/dev/null 2>&1 && echo g)sed

MXE_SYSTEM := msvc

ifeq ($(MXE_SYSTEM),msvc)
  CL_VERSION :=   $(shell cl -? 2>&1 | $(SED) -n -e 's/.*Compiler Version \([0-9]\+\).*/\1/p')
  MSVC_VERSION := $(if $(filter 17,$(CL_VERSION)),110, \
		  $(if $(filter 16,$(CL_VERSION)),100, \
		  $(if $(filter 15,$(CL_VERSION)),90)))
  LIBRARY_PREFIX := vc$(strip $(MSVC_VERSION))-lib
  CONFIGURE_POST_HOOK := lt-postproc -p '$(LIBRARY_PREFIX)' -s '$(LIBRARY_SUFFIX)'
  MAKE_SHARED_FROM_STATIC_OPTIONS := --windowsdll --msvc --libprefix '$(LIBRARY_PREFIX)' --libsuffix '$(LIBRARY_SUFFIX)'
endif


MXE_NATIVE_BUILD := yes

MXE_SYSTEM := msvcs

ifneq ($(MXE_NATIVE_BUILD),yes)
  ifeq ($(MXE_SYSTEM),msvc)
    $(error Cross-compiling to MSVC is not supported)
  endif
  HOST_AND_BUILD_CONFIGURE_OPTIONS := \
    --host='$(TARGET)' --build='$(BUILD_SYSTEM)'
endif


SHELL := bash

INSTALL    := $(shell ginstall --help >/dev/null 2>&1 && echo g)install
PATCH      := $(shell gpatch --help >/dev/null 2>&1 && echo g)patch
SED        := $(shell gsed --help >/dev/null 2>&1 && echo g)sed
SORT       := $(shell gsort --help >/dev/null 2>&1 && echo g)sort
WGET       := wget  \
                   --user-agent=$(shell wget --version | \
                   $(SED) -n 's,GNU \(Wget\) \([0-9.]*\).*,\1/\2,p')



# Should match what config.guess prints for your system.
# If cross compiling, you must set it manually (defaults are set by configure).
ifeq ($(MXE_NATIVE_BUILD),yes)
  TARGET := $(shell $(TOP_DIR)/tools/config.guess)
else
  TARGET := i686-w64-mingw32
endif
BUILD_SYSTEM := $(shell $(TOP_DIR)/tools/config.guess)


DATE := $(shell date +%Y-%m-%d-%H-%M)



# Current valid values are mingw (cross or native), msvc (native)
# and gnu-linux (native).
MXE_SYSTEM := mingws
ifeq ($(MXE_SYSTEM),mingw)
else ifeq ($(MXE_SYSTEM),msvc)
else ifeq ($(MXE_SYSTEM),gnu-linux)
else
$(error "$(MXE_SYSTEM)" is not a valid value for MXE_SYSTEM)
endif


BUILD_SHARED := yes

BUILD_STATIC := yes

ifeq ($(BUILD_SHARED),yes)
  ifeq ($(BUILD_STATIC),yes)
    ENABLE_SHARED_OR_STATIC := --enable-shared --enable-static
    WITH_SHARED_OR_STATIC := --with-shared --with-static
  else
    ENABLE_SHARED_OR_STATIC := --enable-shared --disable-static
    WITH_SHARED_OR_STATIC := --with-shared --without-static
  endif
else
  ENABLE_SHARED_OR_STATIC := --disable-shared --enable-static
  WITH_SHARED_OR_STATIC := --without-shared --with-static
endif


BUILD_TOOLS := msbuilds

BUILD_TOOLS := $(filter-out build-python, $(BUILD_TOOLS))

OCTAVE_TARGET := default-octave

PWD := $(shell pwd)
TOP_BUILD_DIR := /f/Application/MSBuildSolutions/mxe-octave
TOP_DIR := /f/Application/MSBuildSolutions/mxe-octave

STRIP_DIST_FILES := yes

DATE := $(shell date +%Y-%m-%d-%H-%M)

## Configuration variables.

# Set the following configuration variables with a configure script?

# Current valid values are mingw (cross or native), msvc (native)
# and gnu-linux (native).
MXE_SYSTEM := mingw
ifeq ($(MXE_SYSTEM),mingw)
else ifeq ($(MXE_SYSTEM),msvc)
else ifeq ($(MXE_SYSTEM),gnu-linux)
else
$(error "$(MXE_SYSTEM)" is not a valid value for MXE_SYSTEM)
endif

# Set to "no" if doing a cross compile build.
MXE_NATIVE_BUILD := no

# Set to "yes" to use ccache
USE_CCACHE := no

# Set to "yes" to use the versions of GCC and binutils already
# installed on your system (default is yes if doing native build).
USE_SYSTEM_GCC := no

# Set to "yes" to use the version of fontconfig already installed
# on your system (default is yes if doing native build). 
USE_SYSTEM_FONTCONFIG := no

# Set to "yes" to use the X11 libraries already installed
# on your system (default is yes if doing native build). 
USE_SYSTEM_X11_LIBS := no

# Build compiler for 64-bit Windows.  Using this option will build
# binaries that require a 64-bit Windows system.
ENABLE_WINDOWS_64 := no

# Attempt to build Octave and dependencies with 64-bit indexing
# enabled (experimental; requires 64-bit system and compiler).
# Note that 64-bit indexing is NOT required simply because you have
# 64-bit operating system.  Building Octave with 64-bit indexing is
# still experimental, so enabling this option is discouraged unless
# you know what you are doing.
ENABLE_64 := no

# create binary octave forge tools.
# if cross, compiling, this also means we will attempt to create cross mkoctfile
ENABLE_BINARY_PACKAGES := no

# create and install devel tools (default no)
ENABLE_DEVEL_TOOLS := no

# Should match what config.guess prints for your system.
# If cross compiling, you must set it manually (defaults are set by configure).
ifeq ($(MXE_NATIVE_BUILD),yes)
  TARGET := $(shell $(TOP_DIR)/tools/config.guess)
else
  TARGET := i686-w64-mingw32
endif
BUILD_SYSTEM := $(shell $(TOP_DIR)/tools/config.guess)

# Enable shared or static libs, or perhaps both.  Probably it doesn't
# make sense to disable both...
BUILD_SHARED := yes
BUILD_STATIC := no

USE_PIC_FLAG := no

# Compile Qhull with -fno-strict-aliasing flag.
ENABLE_QHULL_NO_STRICT_ALIASING_FLAG := no

# Attempt to build with support for Java.
ENABLE_JAVA := yes

# Attempt to build LLVM and enable Octave's experimental JIT compiler.
ENABLE_JIT := no

# Build octave docs 
ENABLE_DOCS := yes

# Use openblas for octave
ENABLE_OPENBLAS := yes

# Look in lib64 directory.
MXE_USE_LIB64_DIRECTORY := no

# Do not remove the temporary build directories. Set to 1 to keep them
# around after the build step.
KEEP_BUILD :=

## end of configuration variables.

ifneq ($(MXE_NATIVE_BUILD),yes)
  ifeq ($(MXE_SYSTEM),msvc)
    $(error Cross-compiling to MSVC is not supported)
  endif
  HOST_AND_BUILD_CONFIGURE_OPTIONS := \
    --host='$(TARGET)' --build='$(BUILD_SYSTEM)'
endif

# are we doing a native mingw build ?
ifeq ($(MXE_NATIVE_BUILD),yes)
  ifeq ($(MXE_SYSTEM),mingw)
    MXE_NATIVE_MINGW_BUILD := yes
  endif
  ifeq ($(MXE_SYSTEM),msvc)
    MXE_NATIVE_MINGW_BUILD := yes
  endif
endif

# These can't be chosen arbitrarily.  The way things are configured now,
# GCC expects to find cross-compiler include files in $(PREFIX)/$(TARGET).
# and it's not clear to me how to change that.
#
# For native builds, dump everything all together in one directory tree.
ROOT_PREFIX := $(TOP_BUILD_DIR)/usr
BUILD_TOOLS_PREFIX := $(ROOT_PREFIX)
ifeq ($(MXE_NATIVE_BUILD),yes)
  HOST_PREFIX := $(ROOT_PREFIX)
else
  HOST_PREFIX := $(ROOT_PREFIX)/$(TARGET)
endif

# Make sure the root installation directories exists
$(shell mkdir -p $(ROOT_PREFIX))
$(shell mkdir -p $(BUILD_TOOLS_PREFIX))
$(shell mkdir -p $(HOST_PREFIX))

# At least one package uses --with instead of --enable.  
ifeq ($(BUILD_SHARED),yes)
  ifeq ($(BUILD_STATIC),yes)
    ENABLE_SHARED_OR_STATIC := --enable-shared --enable-static
    WITH_SHARED_OR_STATIC := --with-shared --with-static
  else
    ENABLE_SHARED_OR_STATIC := --enable-shared --disable-static
    WITH_SHARED_OR_STATIC := --with-shared --without-static
  endif
else
  ENABLE_SHARED_OR_STATIC := --disable-shared --enable-static
  WITH_SHARED_OR_STATIC := --without-shared --with-static
endif

ifeq ($(USE_PIC_FLAG),yes)
  MXE_CC_PICFLAG := -fPIC
  MXE_CXX_PICFLAG := -fPIC
  MXE_F77_PICFLAG := -fPIC
endif

JOBS               := 1
SOURCEFORGE_MIRROR := freefr.dl.sourceforge.net
PKG_MIRROR         := s3.amazonaws.com/mxe-pkg
PKG_CDN            := d1yihgixbnrglp.cloudfront.net

SHELL := bash

INSTALL    := $(shell ginstall --help >/dev/null 2>&1 && echo g)install
PATCH      := $(shell gpatch --help >/dev/null 2>&1 && echo g)patch
SED        := $(shell gsed --help >/dev/null 2>&1 && echo g)sed
SORT       := $(shell gsort --help >/dev/null 2>&1 && echo g)sort
WGET       := wget  \
                   --user-agent=$(shell wget --version | \
                   $(SED) -n 's,GNU \(Wget\) \([0-9.]*\).*,\1/\2,p')
# The MSYS GNU tar does not support long uid/gid, leading to unpacking
# errors for some archives. Bsdtar does not have that limitation, so
# use it on Win32 platform if GNU tar fails to unpack the archive.
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
    TAR  := tar
    TAR2 := bsdtar
else
    TAR  := tar
endif

REQUIREMENTS := bash bzip2 $(MAKE) openssl $(PATCH) $(PERL) \
                $(SED) tar unzip wget

ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += bsdtar
endif

LIBTOOL     := libtool
LIBTOOLIZE  := libtoolize

## Build tools are tools that we need to build everything else.
## They run on the build system.  Some, like gcc and binutils may
## generate output that runs on the host system (TARGET in MXE terms).
ALL_BUILD_TOOLS := $(shell $(SED) -n 's/^.* class="package">\(build-[^<]*\)<.*$/\1/p' '$(TOP_DIR)/index.html')

BUILD_COMPILER_TOOLS := build-gcc build-binutils
BUILD_TOOLS := $(ALL_BUILD_TOOLS)
# Building flex for native mingw fails, so disable it.
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += flex
  BUILD_TOOLS := $(filter-out build-flex, $(BUILD_TOOLS))
endif
# Building bison for native mingw creates a bison that doesnt  
# allow push-pull mode so disable
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += bison
  BUILD_TOOLS := $(filter-out build-bison, $(BUILD_TOOLS))
endif
# use the msys m4 in native mingw
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += m4
  BUILD_TOOLS := $(filter-out build-m4, $(BUILD_TOOLS))
endif
# Use texinfo and xz from msys under MSVC.
ifeq ($(MXE_SYSTEM),msvc)
  REQUIREMENTS += makeinfo xz
  BUILD_TOOLS := $(filter-out build-texinfo build-xz, $(BUILD_TOOLS))
endif
# use the a native python in native mingw
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += python
  BUILD_TOOLS := $(filter-out build-python, $(BUILD_TOOLS))
endif

# use the a native awk in native mingw
ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
  REQUIREMENTS += awk
  BUILD_TOOLS := $(filter-out build-gawk, $(BUILD_TOOLS))
endif

ifeq ($(USE_SYSTEM_GCC),yes)
  BUILD_TOOLS := $(filter-out $(BUILD_COMPILER_TOOLS), $(BUILD_TOOLS))
endif
ifneq ($(MXE_SYSTEM),msvc)
  BUILD_TOOLS := $(filter-out build-msvctools, $(BUILD_TOOLS))
endif

STAMP_DIR  := $(TOP_BUILD_DIR)/installed-packages
BUILT_PKGS_DIR := $(TOP_BUILD_DIR)/built-packages
MSYS_INFO_DIR := $(TOP_BUILD_DIR)/msys-info
LOG_DIR    := $(TOP_BUILD_DIR)/log
TIMESTAMP  := $(shell date +%Y%m%d_%H%M%S)
PKG_DIR    := /f/Application/MSBuildSolutions/mxe-octave/pkg
TMP_DIR     = $(TOP_BUILD_DIR)/tmp-$(1)
MAKEFILE   := $(TOP_BUILD_DIR)/Makefile
PKGS       := $(filter-out $(ALL_BUILD_TOOLS), $(shell $(SED) -n 's/^.* class="package">\([^<]*\)<.*$/\1/p' '$(TOP_DIR)/index.html'))

ENV_PATH := $(PATH)
PATH := $(BUILD_TOOLS_PREFIX)/bin:$(PATH)
ifeq ($(USE_CCACHE),yes)
  PATH := $(BUILD_TOOLS_PREFIX)/bin/ccache:$(PATH)
  CCACHE_LINKS_TARGET := ccache-links
endif

ENV_PKG_CONFIG_PATH := $(PKG_CONFIG_PATH)

CONFIGURE_CPPFLAGS := CPPFLAGS='-I$(HOST_PREFIX)/include'
ifeq ($(MXE_USE_LIB64_DIRECTORY),yes)
  CONFIGURE_LDFLAGS := LDFLAGS='-L$(HOST_PREFIX)/lib -L$(HOST_PREFIX)/lib64'
  PKG_CONFIG_PATH := $(HOST_PREFIX)/lib64/pkgconfig
  export PKG_CONFIG_PATH
else
  CONFIGURE_LDFLAGS := LDFLAGS='-L$(HOST_PREFIX)/lib'
  PKG_CONFIG_PATH := $(HOST_PREFIX)/lib/pkgconfig
  export PKG_CONFIG_PATH
endif
CONFIGURE_POST_HOOK := true

HOST_BINDIR := $(HOST_PREFIX)/bin
HOST_LIBDIR := $(HOST_PREFIX)/lib
HOST_LIB64DIR := $(HOST_PREFIX)/lib64
HOST_INCDIR := $(HOST_PREFIX)/include

CMAKE_HOST_PREFIX := $(HOST_PREFIX)
CMAKE_ROOT_PREFIX := $(ROOT_PREFIX)

ifeq ($(MXE_NATIVE_BUILD),yes)
  MXE_TOOL_PREFIX := 
  ifeq ($(MXE_NATIVE_MINGW_BUILD),yes)
    # The installation directories have been created previously. So we
    # can rely on their existence to compute their Win32 equivalent.
    HOST_PREFIX_NATIVE := $(shell cd $(HOST_PREFIX) && pwd -W)
    ROOT_PREFIX_NATIVE := $(shell cd $(ROOT_PREFIX) && pwd -W)
    # CMake uses Win32 paths, not MSYS paths. When using a MSYS path as
    # DESTDIR, CMake translates it into a Win32 path and strip the
    # drive letter from it.
    CMAKE_HOST_PREFIX := $(shell echo $(HOST_PREFIX_NATIVE) | $(SED) -e 's/^[a-zA-Z]://')
    CMAKE_ROOT_PREFIX := $(shell echo $(ROOT_PREFIX_NATIVE) | $(SED) -e 's/^[a-zA-Z]://')
  endif
  ifeq ($(MXE_SYSTEM),msvc)
    MXE_AR := ar
    MXE_RANLIB := true
    MXE_CC := clgcc
    MXE_CCAS := gcc
    MXE_CXX := clg++
    MXE_F77 := clgfortran
    MXE_DLLTOOL := true
    MXE_NM := nm
    MXE_STRIP := true
    MXE_WINDRES := windres
    MXE_PKG_CONFIG := pkg-config
    MXE_QMAKE := qmake
    # Although the Fortran compiler is internally gfortran, we still
    # want to make it look like non-GNU compiler, otherwise libtool is
    # not playing nicely and trips over different variable values for
    # different configuration tags.
    HOST_AND_BUILD_CONFIGURE_OPTIONS := \
	    CC='$(MXE_CC)' CXX='$(MXE_CXX)' F77='$(MXE_F77)' DLLTOOL='$(MXE_DLLTOOL)' CCAS='$(MXE_CCAS)' \
	    CFLAGS='-O2' CXXFLAGS='-O2' ac_cv_f77_compiler_gnu=no RANLIB='$(MXE_RANLIB)'
    # CLGCC wrappers automatically adjust INCLUDE and LIB variables.
    CONFIGURE_CPPFLAGS :=
    CONFIGURE_LDFLAGS :=
  else
    MXE_AR := ar
    MXE_RANLIB := ranlib
    MXE_CC := gcc
    MXE_CCAS := gcc
    MXE_CXX := g++
    MXE_F77 := gfortran
    MXE_DLLTOOL := dlltool
    MXE_NM := nm
    MXE_STRIP := strip
    ifeq ($(MXE_SYSTEM),mingw)
      MXE_WINDRES := windres
    else
      MXE_WINDRES := true
    endif
    MXE_PKG_CONFIG := pkg-config
    MXE_QMAKE := qmake
  endif

  MXE_MOC := $(HOST_BINDIR)/moc
  MXE_UIC := $(HOST_BINDIR)/uic
  MXE_LRELEASE := $(HOST_BINDIR)/lrelease
  MXE_RCC := $(HOST_BINDIR)/rcc

  MXE_MKOCTFILE := $(HOST_BINDIR)/mkoctfile
  MXE_OCTAVECONFIG := $(HOST_BINDIR)/octave-config
else
  MXE_TOOL_PREFIX := $(TARGET)-
  MXE_AR := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)ar'
  MXE_RANLIB := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)ranlib'
  MXE_CC := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)gcc'
  MXE_CCAS := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)gcc'
  MXE_CXX := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)g++'
  MXE_F77 := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)gfortran'
  MXE_DLLTOOL := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)dlltool'
  MXE_NM := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)nm'
  MXE_STRIP := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)strip'
  ifeq ($(MXE_SYSTEM),mingw)
    MXE_WINDRES := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)windres'
  else
    MXE_WINDRES := true
  endif
  MXE_PKG_CONFIG := '$(BUILD_TOOLS_PREFIX)/bin/pkg-config'
  MXE_QMAKE := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)qmake'
  MXE_MOC := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)moc'
  MXE_UIC := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)uic'
  MXE_LRELEASE := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)lrelease'
  MXE_RCC := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)rcc'

  MXE_MKOCTFILE := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)mkoctfile'
  MXE_OCTAVECONFIG := '$(BUILD_TOOLS_PREFIX)/bin/$(MXE_TOOL_PREFIX)octave-config'
endif

ifeq ($(MXE_SYSTEM),mingw)
  MAKE_SHARED_FROM_STATIC_OPTIONS := --windowsdll
endif

ENV_LD_LIBRARY_PATH := $(LD_LIBRARY_PATH)

ifeq ($(MXE_SYSTEM),mingw)
  MXE_WINDOWS_BUILD := yes
  ifneq ($(MXE_NATIVE_BUILD),yes)
    MSYS_URL := http://sourceforge.net/projects/mingw/files/MSYS
    MSYS_BASE_URL := $(MSYS_URL)/Base
    MSYS_EXTENSION_URL := $(MSYS_URL)/Extension
    MSYS_BASE_VER := 1.0.13
    MSYS_EXTENSION_VER := 1.0.13
    MSYS_BASE_DIR := $(TOP_BUILD_DIR)/msys-base
    MSYS_EXTENSION_DIR := $(TOP_BUILD_DIR)/msys-extension
    MSYS_BASE_PACKAGES := $(addprefix msys-,bash coreutils diffutils dos2unix file findutils gawk grep gzip less libcrypt libiconv libintl libmagic libopenssl make msys-core patch perl regex sed tar termcap unzip zip wget zlib)

    MSYS_BASE_SOURCES := $(addprefix src-,$(MSYS_BASE_PACKAGES))
    PKGS += $(MSYS_BASE_SOURCES)

     NOTEPAD_BASE_DIR := $(TOP_BUILD_DIR)/notepad++
  endif
else ifeq ($(MXE_SYSTEM),msvc)
  MXE_WINDOWS_BUILD := yes
else
  MXE_WINDOWS_BUILD := no
  MXE_CPPFLAGS := '-I$(HOST_INCDIR)'
  ifeq ($(MXE_USE_LIB64_DIRECTORY),yes)
    MXE_LDFLAGS := '-L$(HOST_LIBDIR) -L$(HOST_LIB64DIR)'
    LD_LIBRARY_PATH := $(HOST_LIBDIR):$(HOST_LIB64DIR)
  else
    MXE_LDFLAGS := '-L$(HOST_LIBDIR)'
    LD_LIBRARY_PATH := $(HOST_LIBDIR)
  endif
  export LD_LIBRARY_PATH
endif

LN := ln
LN_S := $(LN) -s
LN_SF := $(LN_S) -f
ifeq ($(MXE_NATIVE_BUILD),yes)
  ifeq ($(MXE_SYSTEM),mingw)
    LN := cp
    LN_S := $(LN)
    LN_SF := $(LN_S)
  endif
  ifeq ($(MXE_SYSTEM),msvc)
    LN := cp
    LN_S := $(LN)
    LN_SF := $(LN_S)
  endif
endif

LIBRARY_PREFIX :=
LIBRARY_SUFFIX :=

ifeq ($(MXE_SYSTEM),msvc)
  CL_VERSION := $(shell cl -? 2>&1 | $(SED) -n -e 's/.*Compiler Version \([0-9]\+\).*/\1/p')
  MSVC_VERSION := $(if $(filter 17,$(CL_VERSION)),110, \
		  $(if $(filter 16,$(CL_VERSION)),100, \
		  $(if $(filter 15,$(CL_VERSION)),90)))
  LIBRARY_PREFIX := vc$(strip $(MSVC_VERSION))-lib
  CONFIGURE_POST_HOOK := lt-postproc -p '$(LIBRARY_PREFIX)' -s '$(LIBRARY_SUFFIX)'
  MAKE_SHARED_FROM_STATIC_OPTIONS := --windowsdll --msvc --libprefix '$(LIBRARY_PREFIX)' --libsuffix '$(LIBRARY_SUFFIX)'
endif

OCTAVE_FORGE_BASE_URL := 'http://sourceforge.net/projects/octave/files/Octave Forge Packages/Individual Package Releases'
OCTAVE_FORGE_PACKAGES := $(addprefix of-,miscellaneous struct optim specfun general signal communications image io statistics control geometry windows linear-algebra sockets zenity data-smoothing fuzzy-logic-toolkit quaternion fits tsa dicom netcdf ltfat database instrument-control generate_html nurbs strings financial stk splines dataframe lssa queueing ga odepkg interval nan ocs mapping tisean video zeromq)
ifeq ($(ENABLE_64),no)
  ## This package does not work properly with 64-bit indexing
  OCTAVE_FORGE_PACKAGES += of-sparsersb
endif
# get ALL deps for all of- packages, regardless of whether building the actual forge package in the installer
# OCTAVE_FORGE_DEPS:= $(sort $(foreach p,$(addprefix $(TOP_DIR)/src/,$(addsuffix .mk,$(OCTAVE_FORGE_PACKAGES))),$(shell $(SED) -n 's/.*_DEPS.*:=\(.*\)/\1/p' $p)))

BLAS_PACKAGES := blas
ifeq ($(ENABLE_OPENBLAS),yes)
  BLAS_PACKAGES += openblas
endif

DEVEL_PACKAGES := 
ifeq ($(ENABLE_DEVEL_TOOLS),yes)
  DEVEL_PACKAGES += gdb
endif


MAKE_SHARED_FROM_STATIC := \
  $(TOP_DIR)/tools/make-shared-from-static $(MAKE_SHARED_FROM_STATIC_OPTIONS)

CMAKE_TOOLCHAIN_FILE := $(HOST_PREFIX)/share/cmake/mxe-conf.cmake

# unexport any environment variables that might cause trouble
unexport AR CC CFLAGS C_INCLUDE_PATH CPATH CPLUS_INCLUDE_PATH CPP F77 FFLAGS
unexport CPPFLAGS CROSS CXX CXXCPP CXXFLAGS EXEEXT EXTRA_CFLAGS
unexport EXTRA_LDFLAGS LD LDFLAGS LIBRARY_PATH LIBS NM
unexport OBJC_INCLUDE_PATH PKG_CONFIG QMAKESPEC RANLIB STRIP

SHORT_PKG_VERSION = \
    $(word 1,$(subst ., ,$($(1)_VERSION))).$(word 2,$(subst ., ,$($(1)_VERSION)))

UNPACK_ARCHIVE = \
    $(if $(filter %.tgz,     $(1)),$(3) xzf '$(1)', \
    $(if $(filter %.tar.gz,  $(1)),$(3) xzf '$(1)', \
    $(if $(filter %.tar.bz2, $(1)),$(3) xjf '$(1)', \
    $(if $(filter %.tar.lzma,$(1)),xz -dc -F lzma '$(1)' | $(3) xf -, \
    $(if $(filter %.tar.xz,$(1)),xz -dc '$(1)' | $(3) xf -, \
    $(if $(filter %.zip,     $(1)),unzip -q '$(1)', \
    $(error Unknown archive format for $(2): $(1))))))))

UNPACK_PKG_ARCHIVE = \
    $(if $(value $(1)_FILE),$(call UNPACK_ARCHIVE,$(PKG_DIR)/$($(1)_FILE),$(1),$(2)),true)

PKG_CHECKSUM = \
    openssl sha1 '$(PKG_DIR)/$($(1)_FILE)' 2>/dev/null | $(SED) -n 's,^.*\([0-9a-f]\{40\}\)$$,\1,p'

CHECK_PKG_ARCHIVE = \
    [ -f '$(PKG_DIR)/$($(1)_FILE)' -a \( -z "$($(1)_CHECKSUM)" -o '$($(1)_CHECKSUM)' == "`$$(call PKG_CHECKSUM,$(1))`" \) ]

DOWNLOAD_PKG_ARCHIVE = \
    mkdir -p '$(PKG_DIR)' && \
    $(if $($(1)_URL_2), \
        ( $(WGET) -T 30 -t 3 -O- '$($(1)_URL)' || \
          $(WGET) -O- '$($(1)_URL_2)' || \
          $(WGET) -O- '$(PKG_MIRROR)/$($(1)_FILE)' || \
          $(WGET) -O- '$(PKG_CDN)/$($(1)_FILE)' ), \
        ( $(WGET) -O- '$($(1)_URL)' || \
          $(WGET) -O- '$(PKG_MIRROR)/$($(1)_FILE)' || \
          $(WGET) -O- '$(PKG_CDN)/$($(1)_FILE)' )) \
    $(if $($(1)_FIX_GZIP), \
        | gzip -d | gzip -9n, \
        ) \
    > '$(PKG_DIR)/$($(1)_FILE)' || rm -f '$(PKG_DIR)/$($(1)_FILE)'

ifeq ($(IGNORE_SETTINGS),yes)
    $(info [ignore settings.mk])
else ifeq ($(wildcard $(TOP_BUILD_DIR)/settings.mk),$(TOP_BUILD_DIR)/settings.mk)
    include $(TOP_BUILD_DIR)/settings.mk
else
    $(info [create settings.mk])
    $(shell { \
        echo '#JOBS = $(JOBS)'; \
        echo '#PKGS ='; \
    } >'$(TOP_BUILD_DIR)/settings.mk')
endif

.PHONY: all
all:
	$(MAKE) prerequisites
	$(MAKE) all-targets

.PHONY: prerequisites
prerequisites: $(TOP_DIR)/configure Makefile $(CCACHE_LINKS_TARGET)

.PHONY: ccache-links
ccache-links:
	@echo "creating ccache links"
	@mkdir -p usr/bin/ccache && \
	for f in gcc g++ gfortran; do \
	  if test ! -e usr/bin/ccache/$(MXE_TOOL_PREFIX)$$f; then \
	    ln -s /usr/bin/ccache usr/bin/ccache/$(MXE_TOOL_PREFIX)$$f; \
	  fi; \
	done && \
	for f in gcc g++ gfortran; do \
	  if test ! -e usr/bin/ccache/$$f; then \
	    ln -s /usr/bin/ccache usr/bin/ccache/$$f; \
	  fi; \
	done

.PHONY: all-targets
all-targets: $(OCTAVE_TARGET)

.PHONY: all-packages
all-packages: $(PKGS)

$(PKGS): $(BUILD_TOOLS) cmake-toolchain-file.stamp

.PHONY: msys-base
msys-base:  $(MSYS_BASE_PACKAGES)

.PHONY: msys-base-sources
msys-base-sources:  $(MSYS_BASE_SOURCES)

.PHONY: octave-forge-packages
octave-forge-packages: $(OCTAVE_FORGE_DEPS) $(OCTAVE_FORGE_PACKAGES)

.PHONY: blas-packages
blas-packages: $(BLAS_PACKAGES)

.PHONY: devel-packages
devel-packages: $(DEVEL_PACKAGES)

$(TOP_DIR)/configure: $(TOP_DIR)/configure.ac
	cd $(TOP_DIR) && autoconf

config.status: $(TOP_DIR)/configure
	$(SHELL) ./config.status --recheck

Makefile: $(TOP_DIR)/Makefile.in config.status
	$(SHELL) ./config.status Makefile

.PHONY: check-requirements
define CHECK_REQUIREMENT
    @if ! $(1) --help &>/dev/null; then \
        echo; \
        echo 'Missing requirement: $(1)'; \
        echo; \
        echo 'Please have a look at "index.html" to ensure'; \
        echo 'that your system meets all requirements.'; \
	exit 1; \
        echo; \
    fi

endef
define CHECK_REQUIREMENT_VERSION
    @if ! $(1) --version | head -1 | grep ' \($(2)\)$$' >/dev/null; then \
        echo; \
        echo 'Wrong version of requirement: $(1)'; \
        echo; \
        echo 'Please have a look at "index.html" to ensure'; \
        echo 'that your system meets all requirements.'; \
        echo; \
        exit 1; \
    fi

endef
check-requirements: $(STAMP_DIR)/check-requirements
$(STAMP_DIR)/check-requirements: $(MAKEFILE)
	@echo '[check requirements]'
	$(foreach REQUIREMENT,$(REQUIREMENTS),$(call CHECK_REQUIREMENT,$(REQUIREMENT)))
	@[ -d '$(STAMP_DIR)' ] || mkdir -p '$(STAMP_DIR)'
	@if test "$(USE_SYSTEM_GCC)" = yes; then \
	  $(INSTALL) -d '$(BUILD_TOOLS_PREFIX)/bin' ; \
	  $(INSTALL) -m 755 $(TOP_DIR)/tools/config.guess '$(BUILD_TOOLS_PREFIX)/bin/config.guess' ; \
	  $(INSTALL) -m 755 $(TOP_DIR)/tools/config.sub '$(BUILD_TOOLS_PREFIX)/bin/config.sub' ; \
	fi
	@touch '$@'

define newline


endef

include $(patsubst %,$(TOP_DIR)/src/%.mk,$(ALL_BUILD_TOOLS))
include $(patsubst %,$(TOP_DIR)/src/%.mk,$(PKGS))

.PHONY: download
download: $(addprefix download-,$(PKGS)) $(addprefix download-,$(BUILD_TOOLS))

GENERATE_DIR_FILE_LIST = \
    cd '$(1)' && find . -mindepth 1 -printf '%M %P\n'

define GENERATE_FILE_LIST
    [ -d '$(STAMP_DIR)' ] || mkdir -p '$(STAMP_DIR)'; \
	echo -n > '$(STAMP_DIR)/$(1).files'; \
	echo -n > '$(STAMP_DIR)/$(1).pkg'; \
	if [ -d '$(2)$(HOST_PREFIX)' ]; then \
		($(call GENERATE_DIR_FILE_LIST,$(2)$(HOST_PREFIX))) >> '$(STAMP_DIR)/$(1).files'; \
	fi; \
	if [ "$(HOST_PREFIX)" != "$(CMAKE_HOST_PREFIX)" ] && [ -d '$(2)$(CMAKE_HOST_PREFIX)' ]; then \
		($(call GENERATE_DIR_FILE_LIST,$(2)$(CMAKE_HOST_PREFIX))) >> '$(STAMP_DIR)/$(1).files'; \
	fi; \
	if [ -d '$(2)$(ROOT_PREFIX)' ]; then \
		($(call GENERATE_DIR_FILE_LIST,$(2)$(ROOT_PREFIX))) >> '$(STAMP_DIR)/$(1).pkg'; \
	fi; \
	if [ "$(ROOT_PREFIX)" != "$(CMAKE_ROOT_PREFIX)" ] && [ -d '$(2)$(CMAKE_ROOT_PREFIX)' ]; then \
		($(call GENERATE_DIR_FILE_LIST,$(2)$(CMAKE_ROOT_PREFIX))) >> '$(STAMP_DIR)/$(1).pkg'; \
	fi
endef

define CREATE_PKG
    [ -d '$(BUILT_PKGS_DIR)' ] || mkdir -p '$(BUILT_PKGS_DIR)'; \
    if [ -d '$(2)$(ROOT_PREFIX)' ]; then \
        (cd '$(2)$(ROOT_PREFIX)' && tar caf '$(BUILT_PKGS_DIR)/$(1).tar.xz' .); \
    fi; \
    if [ "$(ROOT_PREFIX)" != "$(CMAKE_ROOT_PREFIX)" ] && [ -d '$(2)$(CMAKE_ROOT_PREFIX)' ]; then \
        (cd '$(2)$(CMAKE_ROOT_PREFIX)' && tar caf '$(BUILT_PKGS_DIR)/$(1).tar.xz' .); \
    fi
endef

define INSTALL_PKG_TO_DESTDIR
    (cd '$(ROOT_PREFIX)' && tar xaf '$(BUILT_PKGS_DIR)/$(1).tar.xz')
endef

define OCTAVE_FORGE_PKG_COMPILE
  if [ -d '$(1)/src' ]; then \
    if [ -f '$(1)/src/configure' ]; then \
      cd '$(1)/src' && ./configure $(HOST_AND_BUILD_CONFIGURE_OPTIONS) \
        $(CONFIGURE_CPPFLAGS) $(CONFIGURE_LDFLAGS) MKOCTFILE='$(MXE_MKOCTFILE)' \
        OCTAVE_CONFIG='$(MXE_OCTAVECONFIG)' \
        $(4); \
    fi; \
    if [ -f '$(1)/src/Makefile' ]; then \
      $(MAKE) -C '$(1)/src' MKOCTFILE='$(MXE_MKOCTFILE) --verbose' \
        OCTAVE_CONFIG='$(MXE_OCTAVECONFIG)' || exit 1; \
      rm -f $(1)/src/Makefile; \
    fi; \
    if [ -f '$(1)/src/configure' ]; then \
      rm -f $(1)/src/configure; \
    fi; \
    find '$(1)/src' -type f -name "*.o" -exec rm -f {} \; ; \
  fi
endef

define OCTAVE_FORGE_PKG_BUILD_NOCOMPILE
    mkdir -p '$(3)$(HOST_PREFIX)/src'
    cd '$(1)/..' \
      && tar cvfz $($(PKG)_FILE) $($(PKG)_SUBDIR) \
      && $(INSTALL) -m644 '$($(PKG)_FILE)' '$(3)$(HOST_PREFIX)/src'
endef

ifeq ($(MXE_SYSTEM),msvc)
define OCTAVE_FORGE_PKG_BUILD
    $(INSTALL) -d '$(3)$(HOST_PREFIX)/local/octave-$(octave_VERSION)/share/octave'
    $(INSTALL) -d '$(3)$(HOST_PREFIX)/local/octave-$(octave_VERSION)/lib/octave'
    cd '$(1)/..' && tar cvfz $($(PKG)_FILE) $($(PKG)_SUBDIR)
    cd '$(1)/..' && $(HOST_PREFIX)/local/octave-$(octave_VERSION)/bin/octave-cli.exe -H -f -q \
        --eval "more off;" \
        --eval "pkg('prefix', '`cd "$(3)" && pwd -W`$(HOST_PREFIX)/local/octave-$(octave_VERSION)/share/octave/packages', '`cd "$(3)" && pwd -W`$(HOST_PREFIX)/local/octave-$(octave_VERSION)/lib/octave/packages');" \
	--eval "pkg('install', '-verbose', '$($(PKG)_FILE)');"
endef
else
define OCTAVE_FORGE_PKG_BUILD
    if [ $(ENABLE_BINARY_PACKAGES) = no ]; then \
      mkdir -p '$(3)$(HOST_PREFIX)/src'; \
      cd '$(1)/..' \
        && tar cvfz $($(PKG)_FILE) $($(PKG)_SUBDIR) \
        && $(INSTALL) -m644 '$($(PKG)_FILE)' '$(3)$(HOST_PREFIX)/src'; \
    else \
      cd '$(1)/..' && \
      tar cfz $($(PKG)_FILE) $($(PKG)_SUBDIR) && \
         OCTAVE_CONFIG=$(MXE_OCTAVECONFIG) TMP="$(1)/.." \
         MKOCTFILE=$(MXE_MKOCTFILE) MAKE=$(MAKE) CONFIGURE_OPTIONS="$(HOST_AND_BUILD_CONFIGURE_OPTIONS)" $(4) \
         $(TOP_DIR)/tools/pkg-install.py -verbose "$($(PKG)_FILE)"; \
     fi
endef
endif

define PKG_RULE
.PHONY: download-$(1)
download-$(1): $(addprefix download-,$($(1)_DEPS))
	if ! $(call CHECK_PKG_ARCHIVE,$(1)); then \
	    $(call DOWNLOAD_PKG_ARCHIVE,$(1)); \
	    $(call CHECK_PKG_ARCHIVE,$(1)) || { echo 'Wrong checksum!'; exit 1; }; \
	fi

.PHONY: $(1)
$(1): $(STAMP_DIR)/$(1)
$(STAMP_DIR)/$(1): $(TOP_DIR)/src/$(1).mk \
                          $(wildcard $(TOP_DIR)/src/$(1)-*.patch) \
                          $(wildcard $(TOP_DIR)/src/$(MXE_SYSTEM)-$(1)-*.patch) \
                          $(wildcard $(TOP_DIR)/src/$(1)-test*) \
			  $(shell test -d $(TOP_DIR)/src/$(1) && find $(TOP_DIR)/src/$(1) -type f) \
                          $(addprefix $(STAMP_DIR)/,$($(1)_DEPS)) \
                          | check-requirements
	@[ -d '$(LOG_DIR)/$(TIMESTAMP)' ] || mkdir -p '$(LOG_DIR)/$(TIMESTAMP)'
	@if ! $(call CHECK_PKG_ARCHIVE,$(1)); then \
	    echo '[download] $(1)'; \
	    touch '$(LOG_DIR)/$(TIMESTAMP)/$(1)-download'; \
	    ln -sf '$(TIMESTAMP)/$(1)-download' '$(LOG_DIR)/$(1)-download'; \
	    ($(call DOWNLOAD_PKG_ARCHIVE,$(1))) &> '$(LOG_DIR)/$(1)-download'; \
	    if ! $(call CHECK_PKG_ARCHIVE,$(1)); then \
	        echo; \
	        echo 'Wrong checksum of package $(1)!'; \
	        echo '------------------------------------------------------------'; \
	        tail -n 10 '$(LOG_DIR)/$(1)-download' | $(SED) -n '/./p'; \
	        echo '------------------------------------------------------------'; \
	        echo '[log]      $(LOG_DIR)/$(1)-download'; \
	        echo; \
	        exit 1; \
	    fi; \
	fi
	$(if $(value $(1)_BUILD),  @echo '[build]    $(1)' ,)
	@touch '$(LOG_DIR)/$(TIMESTAMP)/$(1)'
	@ln -sf '$(TIMESTAMP)/$(1)' '$(LOG_DIR)/$(1)'
	@if ! (time $(MAKE) -f '$(MAKEFILE)' 'build-only-$(1)') &> '$(LOG_DIR)/$(1)'; then \
	    echo; \
	    echo 'Failed to build package $(1)!'; \
	    echo '------------------------------------------------------------'; \
	    tail -n 10 '$(LOG_DIR)/$(1)' | $(SED) -n '/./p'; \
	    echo '------------------------------------------------------------'; \
	    echo '[log]      $(LOG_DIR)/$(1)'; \
	    echo; \
	    exit 1; \
	fi
	@echo '[done]     $(1)'

.PHONY: build-only-$(1)
build-only-$(1): PKG = $(1)
build-only-$(1):
	$(if $(value $(1)_BUILD),
	    rm -rf   '$(2)' '$(2)-install'
	    mkdir -p '$(2)'
		( cd '$(2)' && $(call UNPACK_PKG_ARCHIVE,$(1),$(TAR)) ) || \
			$(if $(TAR2), \
				 ( rm -rf '$(2)'; mkdir -p '$(2)'; \
				   cd '$(2)' && $(call UNPACK_PKG_ARCHIVE,$(1),$(TAR2)) ), \
			 false )
		test ! -d '$(TOP_DIR)/src/$(1)' || cp -a '$(TOP_DIR)/src/$(1)' '$(2)'
		cd '$(2)/$($(1)_SUBDIR)'
		$(foreach PKG_PATCH,$(sort $(wildcard $(TOP_DIR)/src/$(1)-*.patch)), \
			(cd '$(2)/$($(1)_SUBDIR)' && $(PATCH) -p1 -u) < $(PKG_PATCH))
		$(foreach PKG_PATCH,$(sort $(wildcard $(TOP_DIR)/src/$(MXE_SYSTEM)-$(1)-*.patch)), \
					(cd '$(2)/$($(1)_SUBDIR)' && $(PATCH) -p1 -u) < $(PKG_PATCH))
		$(call $(1)_BUILD,$(2)/$($(1)_SUBDIR),$(TOP_DIR)/src/$(1)-test,$(2)-install)
		if [ -d '$(2)-install' ] ; 
		 then 
			$(call GENERATE_FILE_LIST,$(1),$(2)-install)
			$(call CREATE_PKG,$(1),$(2)-install)
			$(call INSTALL_PKG_TO_DESTDIR,$(1),$(2)-install)
			(du -k -d 0 '$(2)-install' 2>/dev/null || du -k --max-depth 0 '$(2)-install') | \
			$(SED) -n 's/^\(\S*\).*/du: \1 KiB/p'; \
			[ '$(KEEP_BUILD)' = '1' ] || rm -rfv  '$(2)-install'; \
		fi 
		(du -k -d 0 '$(2)' 2>/dev/null || du -k --max-depth 0 '$(2)') | $(SED) -n 's/^\(\S*\).*/du: \1 KiB/p'
	    ,)
	[ -d '$(STAMP_DIR)' ] || mkdir -p '$(STAMP_DIR)'
	echo $($(1)_VERSION) > '$(STAMP_DIR)/$(1)'
endef
$(foreach PKG,$(PKGS),$(eval $(call PKG_RULE,$(PKG),$(call TMP_DIR,$(PKG)))))
$(foreach TOOL,$(BUILD_TOOLS),$(eval $(call PKG_RULE,$(TOOL),$(call TMP_DIR,$(TOOL)))))

.PHONY: clean
clean:
	rm -rf $(call TMP_DIR,*) $(BUILD_TOOLS_PREFIX)
	rm -rf $(STAMP_DIR) $(MSYS_INFO_DIR) $(LOG_DIR)
	rm -rf $(MSYS_BASE_DIR) $(MSYS_EXTENSION_DIR) $(NOTEPAD_BASE_DIR)
	rm -rf native-tools cross-tools octave gnuplot
	rm -f Makefile cmake-toolchain-file.stamp
	rm -f tools/set-mxe-env.sh

.PHONY: realclean
realclean: clean
	rm -rf $(BUILT_PKGS_DIR)

.PHONY: clean-pkg
clean-pkg:
	rm -f $(patsubst %,'%', \
                  $(filter-out \
                      $(foreach PKG,$(PKGS),$(PKG_DIR)/$($(PKG)_FILE)), \
                      $(wildcard $(PKG_DIR)/*)))

.PHONY: update
define UPDATE
    $(if $(2),
        $(if $(filter $(2),$($(1)_IGNORE)),
            $(info IGNORED  $(1)  $(2)),
            $(if $(filter $(2),$($(1)_VERSION)),
                $(info .        $(1)  $(2)),
                $(info NEW      $(1)  $($(1)_VERSION) --> $(2))
                $(SED) -i 's/^\([^ ]*_VERSION *:=\).*/\1 $(2)/' '$(TOP_DIR)/src/$(1).mk'
                $(MAKE) -f '$(MAKEFILE)' 'update-checksum-$(1)' \
                    || { $(SED) -i 's/^\([^ ]*_VERSION *:=\).*/\1 $($(1)_VERSION)/' '$(TOP_DIR)/src/$(1).mk'; \
                         exit 1; })),
        $(error Unable to update version number of package $(1)))

endef
update:
	$(foreach PKG,$(PKGS),$(call UPDATE,$(PKG),$(shell $($(PKG)_UPDATE))))

update-checksum-%:
	$(call DOWNLOAD_PKG_ARCHIVE,$*)
	$(SED) -i 's/^\([^ ]*_CHECKSUM *:=\).*/\1 '"`$(call PKG_CHECKSUM,$*)`"'/' '$(TOP_DIR)/src/$*.mk'

cleanup-style:
	@$(foreach FILE,$(wildcard $(addprefix $(TOP_DIR)/,Makefile index.html CNAME src/*.mk src/*test.* tools/*)),\
            $(SED) ' \
                s/\r//g; \
                s/[ \t]\+$$//; \
                s,^#!/bin/bash$$,#!/usr/bin/env bash,; \
                $(if $(filter %Makefile,$(FILE)),,\
                    s/\t/    /g; \
                ) \
            ' < $(FILE) > $(TOP_DIR)/tmp-cleanup-style; \
            diff -u $(FILE) $(TOP_DIR)/tmp-cleanup-style >/dev/null \
                || { echo '[cleanup] $(FILE)'; \
                     cp $(TOP_DIR)/tmp-cleanup-style $(FILE); }; \
            rm -f $(TOP_DIR)/tmp-cleanup-style; \
        )

define build-cmake-toolchain-file
    # create the CMake toolchain file
    [ -d '$(dir $(CMAKE_TOOLCHAIN_FILE))' ] || mkdir -p '$(dir $(CMAKE_TOOLCHAIN_FILE))'
    (if [ $(MXE_SYSTEM) = mingw ]; then \
       echo 'set(CMAKE_SYSTEM_NAME Windows)'; \
       echo 'set(MSYS 1)'; \
     fi; \
     if [ $(MXE_SYSTEM) = msvc ]; then \
       echo 'set(MSVC 1)'; \
     fi; \
     if [ $(BUILD_SHARED) = yes ]; then \
       echo 'set(BUILD_SHARED_LIBS ON)'; \
     else \
       echo 'set(BUILD_SHARED_LIBS OFF)'; \
     fi; \
     if [ $(BUILD_STATIC) = yes ]; then \
       echo 'set(BUILD_STATIC_LIBS ON)'; \
     else \
       echo 'set(BUILD_STATIC_LIBS OFF)'; \
     fi; \
     echo 'set(CMAKE_BUILD_TYPE Release)'; \
     if [ $(MXE_SYSTEM) = msvc ]; then \
         echo 'set(CMAKE_FIND_ROOT_PATH $(HOST_PREFIX_NATIVE))'; \
         echo 'set(CMAKE_FIND_ROOT_PATH_MODE_PROGRAM BOTH)'; \
     else \
         echo 'set(CMAKE_FIND_ROOT_PATH $(HOST_PREFIX))'; \
         echo 'set(CMAKE_FIND_ROOT_PATH_MODE_PROGRAM NEVER)'; \
     fi; \
     echo 'set(CMAKE_FIND_ROOT_PATH_MODE_INCLUDE ONLY)'; \
     echo 'set(CMAKE_FIND_ROOT_PATH_MODE_LIBRARY ONLY)'; \
     echo 'set(CMAKE_C_COMPILER $(MXE_CC))'; \
     echo 'set(CMAKE_CXX_COMPILER $(MXE_CXX))'; \
     echo 'set(CMAKE_Fortran_COMPILER $(MXE_F77))'; \
     echo 'set(CMAKE_RC_COMPILER $(MXE_WINDRES))'; \
     echo 'set(PKG_CONFIG_EXECUTABLE $(MXE_PKG_CONFIG))'; \
     echo 'set(QT_QMAKE_EXECUTABLE $(MXE_QMAKE))'; \
     if [ $(MXE_SYSTEM) = msvc ]; then \
         echo 'set(CMAKE_INSTALL_PREFIX $(HOST_PREFIX_NATIVE) CACHE PATH "Installation Prefix")'; \
     else \
         echo 'set(CMAKE_INSTALL_PREFIX $(HOST_PREFIX) CACHE PATH "Installation Prefix")'; \
     fi; \
     echo 'set(CMAKE_BUILD_TYPE Release CACHE STRING "Debug|Release|RelWithDebInfo|MinSizeRel")') \
     > '$(CMAKE_TOOLCHAIN_FILE)'
endef

cmake-toolchain-file.stamp:
	@echo [creating CMake toolchain file]
	@$(build-cmake-toolchain-file)
	@touch cmake-toolchain-file.stamp
	@echo [done]

# Files to distribute
include $(TOP_DIR)/dist-files.mk

PACKAGE_VERSION := 0.0.35
PACKAGE_TARNAME := mxe-octave

distdir := $(PACKAGE_TARNAME)-$(PACKAGE_VERSION)

.PHONY: dist
dist:
	rm -rf $(distdir)
	mkdir $(distdir)
	( cd $(TOP_DIR) ; $(TAR) cf - $(DIST_FILES) ) | ( cd $(distdir) ; $(TAR) xpf - )
	$(TAR) czf $(distdir).tar.gz $(distdir)

include $(TOP_DIR)/binary-dist-rules.mk

include $(TOP_DIR)/hg-octave-dist.mk
