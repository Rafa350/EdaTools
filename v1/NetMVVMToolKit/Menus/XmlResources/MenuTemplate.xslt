<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet 
    version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    
    <xsl:output 
        method="xml" 
        indent="yes" 
        encoding="utf-8"/>

    <xsl:template match="menu">
        <Menu 
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:e="clr-namespace:MikroPic.NetMVVMToolkit.v1.Localization.Extensions;assembly=NetGuiWPF">
            <xsl:apply-templates select="item"/>
        </Menu>
    </xsl:template>
    
    <xsl:template match="item">
        <xsl:element name="MenuItem">
            
            <xsl:attribute name="Header">
                <xsl:text>{e:Translate </xsl:text>
                <xsl:value-of select="@title"/>
                <xsl:text>}</xsl:text>
            </xsl:attribute>
            
            <xsl:if test="string(@command)!=''">
                <xsl:attribute name="Command">
                    <xsl:text>{Binding </xsl:text>
                    <xsl:value-of select="@command"/>
                    <xsl:text>}</xsl:text>
                </xsl:attribute>
            </xsl:if>

            <xsl:if test="@checkable='yes'">
                <xsl:attribute name="IsCheckable">true</xsl:attribute>
            </xsl:if>
            
            <xsl:if test="@checked='yes'">
                <xsl:attribute name="IsChecked">true</xsl:attribute>
            </xsl:if>
            
            <xsl:if test="string(@hint)!=''">
                <xsl:attribute name="ToolTip">
                    <xsl:value-of select="@hint"/>
                </xsl:attribute>
            </xsl:if>
            
            <xsl:if test="string(@icon)!=''">
                <xsl:element name="MenuItem.Icon">
                    <xsl:element name="Image">
                        <xsl:attribute name="Source">
                            <xsl:value-of select="concat('pack://siteoforigin:,,,/',@icon)"/>
                        </xsl:attribute>
                    </xsl:element>
                </xsl:element>
            </xsl:if>
            
            <xsl:apply-templates select="item"/>
            
        </xsl:element>
        
        <xsl:if test="@separator='yes'">
            <xsl:element name="Separator"/>
        </xsl:if>
        
    </xsl:template>
    
</xsl:stylesheet>
